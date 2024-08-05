//using System.Collections;
//using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject PM;
    private GameObject UM;
    private GameObject ES;
    private AudioSource AS;
    private bool isWin = false;
    private bool isDefeat = false;
    EnemySpawner ESS;

    public GameObject Store;
    public GameObject NoBuy;
    public GameObject Buy;
    public float EnemySpeed = 2.0f;
    public int WaveCount = 1;
    public AudioClip DefeatSound;
    public AudioClip WinSound;

    // Start is called before the first frame update
    void Start(){
        PM = GameObject.Find("PlayerArmature");
        UM = GameObject.Find("UIManager");
        ES = GameObject.Find("EnemyGenerator");
        AS = GetComponent<AudioSource>();
        ESS = ES.GetComponent<EnemySpawner>();
    }

    // Update is called once per frame
    void Update(){
        //������Ʈ Ǯ ����� 30�������� 1���̺� 5���� + 5 + 5 + 5 ... 30 ������, 6Wave�� ���̴�.
        if (Input.GetKeyDown(KeyCode.P)) {
            if (!Store.activeSelf)
            {
                Time.timeScale = 0.1f;
                Store.SetActive(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                Buy.SetActive(false);
                NoBuy.SetActive(false);
                Store.SetActive(false);
            }
        }

        if (Store.activeSelf)
        {
            StoreControll();
        }

        if (isDefeat)
        {
            UM.GetComponent<AudioSource>().Stop();
            AS.PlayOneShot(DefeatSound);
        }

        if (isWin)
        {
            UM.GetComponent<AudioSource>().Stop();
            AS.PlayOneShot(WinSound);
        }
        
        if (WaveCount < 7){
            WaveUP();
            UM.GetComponent<UIManager>().SetWaveUI(WaveCount);
            if (PM.GetComponent<PlayerManager>().isPlayerDead)
            {
                GameOver();
            }
        }
        else{ // �̰� 6Wave�� ��ģ �Ŀ� Finished �� ��Ȳ
            GameWin();
        }
    }

    public void WaveUP(){
        //Debug.Log(ESS.CurrentEnemyCount);
        if (ESS.WaveEnemyCount == ESS.DieEnemyCount){
            WaveCount++; //  Wave Up
            ESS.WaveEnemyCount += 5; // �ش� ���̺꿡 ��ƾ� �� ���� 5���� ����
            ESS.CurrentEnemyCount = 0; // ���� �� �ʱ�ȭ
            ESS.DieEnemyCount = 0; // ���� ���� �ʱ�ȭ
            ESS.WaveEnemyAttackStat += 2; // ���ݷ� ����
            ESS.WaveEnemyMaxHP += 5; // ����� HP�� 5�� ����
            EnemySpeed += 0.25f;
            Debug.Log(ESS.WaveEnemyMaxHP); 
        }
    }

    public void StoreControll()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("1");
            if (ESS.Score >= 5)
            {
                ESS.Score -= 5;
                PlayerManager.HPMAX += 30f;
                PlayerManager.CurrentHP += 30f;
                Buy.SetActive(true);
                NoBuy.SetActive(false);
            }
            else
            {
                NoBuy.SetActive(true);
                Buy.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Debug.Log("2");
            if (ESS.Score >= 10)
            {
                ESS.Score -= 10;
                PlayerManager.PlayerBulletPower += 5;
                Buy.SetActive(true);
                NoBuy.SetActive(false);
            }
            else
            {
                NoBuy.SetActive(true);
                Buy.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Debug.Log("3");
            if (ESS.Score >= 10)
            {
                ESS.Score -= 10;
                PlayerManager.MaxBullet += 5;
                PM.GetComponent<PlayerManager>().BulletCount += 5;
                Buy.SetActive(true);
                NoBuy.SetActive(false);
            }
            else
            {
                NoBuy.SetActive(true);
                Buy.SetActive(false);
            }
        }
    }

    public void Restartclick(){
        //Debug.Log("Entered");
        if (Input.GetMouseButtonDown(0)){
            Time.timeScale = 1f; // �ٽ� �ǵ��� ����
            isDefeat = false;
            isWin = false;
            SceneManager.LoadScene("Main");
        }
    }
    
    public void GameOver(){
        isDefeat = true;
        Time.timeScale = 0.1f;
        PM.SetActive(false);
        AS.PlayOneShot(DefeatSound);
        //Debug.Log("GameOver");
        UM.GetComponent<UIManager>().GameOverOBJ.SetActive(true);
        Restartclick();
    }

    public void GameWin()
    {
        isWin = true;
        Time.timeScale = 0f;
        PM.SetActive(false);
        //Debug.Log("GameWin");
        UM.GetComponent<UIManager>().GameWinOBJ.SetActive(true);
        Restartclick();
    }
}
    