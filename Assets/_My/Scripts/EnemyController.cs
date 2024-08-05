using System.Collections;
using System.Collections.Generic;
//using Unity.Properties;
using UnityEngine;
//using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine.AI;
//using UnityEngine.SocialPlatforms.Impl;

public class EnemyController : MonoBehaviour
{
    
    public float enemyMaxHP = 0;
    public int AttackStat = 10;
    private GameObject ES;
    private EnemySpawner ESS;

    private GameObject Player;
    private GameObject GM;
    private bool isDie = false;
    private Transform target;
    
    public NavMeshAgent agent;
    public float enemyCurrentHP = 0;
    public Slider HPBar;
    public Animator EnemyAnimator;
   
    // Start is called before the first frame update
    void Start(){
        EnemyAnimator = GetComponent<Animator>();
        GM = GameObject.Find("GameManager");
        Player = GameObject.Find("PlayerArmature");
        agent = GetComponent<NavMeshAgent>();
        target = Player.transform;
        agent.speed = GM.GetComponent<GameManager>().EnemySpeed;
        InitEnemyHP(); //ó�� ������ ���� ����
        // ����
        ES = GameObject.Find("EnemyGenerator");
        ESS = ES.GetComponent<EnemySpawner>();
        //Debug.Log("����:" + enemyCurrentHP);
        //Debug.Log("Ǯ��:" + enemyMaxHP);
        
    }

    // Update is called once per frame
    void Update(){
        if (!isDie){
            EnemyWalk();
        }
        HPBar.value = enemyCurrentHP / enemyMaxHP; //�÷��̾��� HP�� �� �����Ӹ��� ����
    }

    public void EnemyWalk(){
        agent.SetDestination(target.position);
        EnemyAnimator.SetTrigger("Walk");
        //Debug.Log("walk");
    }

    public void EnemyDamage(){
        EnemyAnimator.SetTrigger("Damage");
        enemyCurrentHP -= PlayerManager.PlayerBulletPower;
    }

    public void EnemyAttack(){
        EnemyAnimator.SetTrigger("Attack");
    }

    void InitEnemyHP(){
        enemyCurrentHP = enemyMaxHP;
    }

    public void EnemyDie(){
        agent.isStopped = true; // ������ Stop ��Ŵ
        if (!isDie)//score+=�� invoke ������ �����ؼ� �� �װ� �����ϱ� ����
        {
            ESS.Score++;
            ESS.DieEnemyCount++;
            //������Ʈ Ǯ�� �ٽ� ���� �ֱ�. ��Ȱ��ȭ
            EnemyAnimator.SetTrigger("Death");
            //Debug.Log("Enemy Die");
            Invoke("AnimewaitObjectPooling", 1f);
        }
        isDie = true;
    }


    void AnimewaitObjectPooling(){
        //Debug.Log("������Ʈ Ǯ�� �ݳ���.");
        EnemySpawner pool = GameObject.Find("EnemyGenerator").GetComponent<EnemySpawner>();
        gameObject.SetActive(false);
        //enemyCurrentHP = enemyMaxHP; //�ҿ����.
        isDie = false;
        pool.EnemyObjectPool.Add(gameObject);
    }

    //���̷����� �÷��̾�� �浹
    public void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == "Player")
        {
            EnemyAttack();
            //Debug.Log("Collision_Attacked");
        }
    }

    //���̷����� �Ѿ˰� �浹 -> �Ѿ��� Trigger Collider ��
    public void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Bullet"){
            EnemyDamage();
            if(enemyCurrentHP <= 0){
                EnemyDie();
            }
            //Debug.Log("Shooted");
        }
    }
}
