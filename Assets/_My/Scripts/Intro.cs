using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject MoveText;
    private float moveTime = 0.1f;
    private float delta = 0f;
    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadingSceneController.Instance.LoadScene("Main");
            Debug.Log("click");
        }

        if(moveTime < delta)//Press SpaceBar To Start 글자에 애니메이션을 주고싶었음
        {
            delta = 0;
            float randx = Random.Range(1f, 1.16f);
            float randy = Random.Range(1f, 1.16f);
            MoveText.transform.localScale = new Vector3(randx, randy, 0);
        }
        delta += Time.deltaTime;        
    }
}
