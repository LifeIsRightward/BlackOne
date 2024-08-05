//using System.Collections;
//using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public Text BulletCountUI;
    public Text ScoreUI;
    public Text TimeUI;
    public float timeCount = 0;
    public Text WaveUI;
    public Text CurrentEnemyUI;
    public GameObject GM;
    public GameObject GameOverOBJ;
    public GameObject GameWinOBJ;

    private GameObject ES;
    private GameObject PM;
    //Ãß°¡
    private EnemySpawner ESS;

    // Start is called before the first frame update
    void Start(){
        PM = GameObject.Find("PlayerArmature");
        GM = GameObject.Find("GameManager");
        ES = GameObject.Find("EnemyGenerator");
        ESS = ES.GetComponent<EnemySpawner>();
        SetWaveUI(GM.GetComponent<GameManager>().WaveCount);
    }

    // Update is called once per frame
    void Update(){
        timeCount += Time.deltaTime;
        SetBulletCountUI();
        SetScoreCountUI();
        SetTimeUI(timeCount);
        SetCurrentEnemyUI();
    }

    public void SetBulletCountUI(){
        BulletCountUI.text = "Bullet X " + PM.GetComponent<PlayerManager>().BulletCount.ToString();
    }
    public void SetScoreCountUI(){
        ScoreUI.text = "Score : " + ESS.Score.ToString();
    }

    public void SetTimeUI(float timeCount){
        
        TimeUI.text = " Time: " + timeCount.ToString("F2");
    }
    public void SetWaveUI(int i){
        WaveUI.text = "Wave: " + i;
    }

    public void SetCurrentEnemyUI(){
        EnemySpawner ESS = ES.GetComponent<EnemySpawner>();
        CurrentEnemyUI.text = "Current Enemy: " + (ESS.WaveEnemyCount - ESS.DieEnemyCount).ToString();
    }
    
}
