using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ����
    // CameManager���� wave���� ����Ǹ� ���⼭ �� �ٲٴ°� �´°� ���⵵,,
    public int Score = 0;
    public int DieEnemyCount = 0;
    public int WaveEnemyAttackStat = 10;
    public float WaveEnemyMaxHP = 10;
    //

    float spawntime = 1.0f;
    float delta = 0;
    int EnemyPoolSize = 30;

    public GameObject EnemyPrefab;
    public List<GameObject> EnemyObjectPool;
    public int WaveEnemyCount; // �ʱ� 1���̺�� 5 ����
    public int CurrentEnemyCount = 0;

    // Start is called before the first frame update
    void Start(){
        WaveEnemyCount = 5;
        EnemyObjectPool = new List<GameObject>();
        for(int i = 0; i < EnemyPoolSize; i++){
            GameObject EnemyObj = Instantiate(EnemyPrefab);
            EnemyObjectPool.Add(EnemyObj);
            EnemyObj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update(){
        this.delta += Time.deltaTime;
        float x = Random.Range(-9, 9);
        float z = Random.Range(-9, 9);
        if(WaveEnemyCount > CurrentEnemyCount){
            if (spawntime < delta){
                 this.delta = 0;
                 CurrentEnemyCount++;
                if (EnemyObjectPool.Count > 0){
                    //Debug.Log("������Ʈ ������.");
                    GameObject EnemyCreate = EnemyObjectPool[0];
                    EnemyObjectPool.Remove(EnemyCreate);
                    EnemyCreate.transform.position = new Vector3(x, 0, z);
                    // ����
                    EnemyController EC = EnemyCreate.GetComponent<EnemyController>();
                    EC.enemyMaxHP = WaveEnemyMaxHP; // max �� ����
                    EC.enemyCurrentHP = WaveEnemyMaxHP; // 
                    EC.AttackStat = WaveEnemyAttackStat; // ���ݷ�

                    EnemyCreate.SetActive(true);
                }
            }
        }
    }
}
