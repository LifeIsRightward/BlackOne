using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;
using System;

public class LoadingSceneController : MonoBehaviour
{
    private static LoadingSceneController instance;

    public static LoadingSceneController Instance // 싱글톤 인스턴스 생성을 위함
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<LoadingSceneController>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = Create();
                }
            }
            return instance;
        }
    }

    private static LoadingSceneController Create()// 프리팹으로 만든 로딩 UI를 불러옴 그리고 리턴함.
    {
        return Instantiate(Resources.Load<LoadingSceneController>("LoadingUI"));
    }

    private void Awake()//싱글톤 객체는 게임에 하나여야 하기때문에 다른곳에서 생성하려고 하거늘, 스스로 자살하게끔 만듦.
    {
        if (instance != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField]
    private CanvasGroup canvasGroup; // 하위에 모든 UI를 관리함.

    [SerializeField]
    private Image progressBar;

    private string loadSceneName;

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded; //씬 로딩이 끝나면 자동으로 OnSceneLoaded을 호출하여 씬 로딩이 끝난 시점을 알려줌. -> 델리게이트에 이벤트 구독
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        
        progressBar.fillAmount = 0f;
        //호출한 코루틴에서 yield로 또 다른 코루틴을 호출시키면 호출한 코루틴이 끝날 때까지 기다리게 할 수 있음.
        yield return StartCoroutine(Fade(true));

        // 페이드인이 끝나면 비동기로 LoadScene을 불러들이는 작업을 함.
        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false; //씬 로딩이 끝나도 자동으로 전환되지 않도록 함.

        float timer = 0f;
        while(!op.isDone) //씬 로딩이 끝나지 않은 상태
        {
            yield return null;
            if(op.progress < 0.9f) //진행도가 90% 이하 인경우 
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer); //1초에 걸쳐 프로그레스바를 채우도록 함.
                if(progressBar.fillAmount >= 1f )
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == loadSceneName) {
            StartCoroutine(Fade(false)); //fadeout
            SceneManager.sceneLoaded -= OnSceneLoaded; //콜백 제거 안하면 씬 로딩될 때 중첩되서 문제 발생함. -> 델리게이트에서 이벤트를 제거해주는듯 함
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            //페이드인이 true이면 fadein, 아니면 fadeout
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f , timer) : Mathf.Lerp (1f, 0f , timer); // 투명도로 Fade in, out 구성
        }

        if(!isFadeIn) //페이드인 과정이 끝난 경우
        {
            gameObject.SetActive(false);
        }

    }
}
