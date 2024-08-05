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

    public static LoadingSceneController Instance // �̱��� �ν��Ͻ� ������ ����
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

    private static LoadingSceneController Create()// ���������� ���� �ε� UI�� �ҷ��� �׸��� ������.
    {
        return Instantiate(Resources.Load<LoadingSceneController>("LoadingUI"));
    }

    private void Awake()//�̱��� ��ü�� ���ӿ� �ϳ����� �ϱ⶧���� �ٸ������� �����Ϸ��� �ϰŴ�, ������ �ڻ��ϰԲ� ����.
    {
        if (instance != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField]
    private CanvasGroup canvasGroup; // ������ ��� UI�� ������.

    [SerializeField]
    private Image progressBar;

    private string loadSceneName;

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded; //�� �ε��� ������ �ڵ����� OnSceneLoaded�� ȣ���Ͽ� �� �ε��� ���� ������ �˷���. -> ��������Ʈ�� �̺�Ʈ ����
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        
        progressBar.fillAmount = 0f;
        //ȣ���� �ڷ�ƾ���� yield�� �� �ٸ� �ڷ�ƾ�� ȣ���Ű�� ȣ���� �ڷ�ƾ�� ���� ������ ��ٸ��� �� �� ����.
        yield return StartCoroutine(Fade(true));

        // ���̵����� ������ �񵿱�� LoadScene�� �ҷ����̴� �۾��� ��.
        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false; //�� �ε��� ������ �ڵ����� ��ȯ���� �ʵ��� ��.

        float timer = 0f;
        while(!op.isDone) //�� �ε��� ������ ���� ����
        {
            yield return null;
            if(op.progress < 0.9f) //���൵�� 90% ���� �ΰ�� 
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer); //1�ʿ� ���� ���α׷����ٸ� ä�쵵�� ��.
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
            SceneManager.sceneLoaded -= OnSceneLoaded; //�ݹ� ���� ���ϸ� �� �ε��� �� ��ø�Ǽ� ���� �߻���. -> ��������Ʈ���� �̺�Ʈ�� �������ִµ� ��
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            //���̵����� true�̸� fadein, �ƴϸ� fadeout
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f , timer) : Mathf.Lerp (1f, 0f , timer); // ������ Fade in, out ����
        }

        if(!isFadeIn) //���̵��� ������ ���� ���
        {
            gameObject.SetActive(false);
        }

    }
}
