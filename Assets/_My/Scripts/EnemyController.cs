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
        InitEnemyHP(); //처음 생성될 때만 적용
        // 수정
        ES = GameObject.Find("EnemyGenerator");
        ESS = ES.GetComponent<EnemySpawner>();
        //Debug.Log("생명:" + enemyCurrentHP);
        //Debug.Log("풀피:" + enemyMaxHP);
        
    }

    // Update is called once per frame
    void Update(){
        if (!isDie){
            EnemyWalk();
        }
        HPBar.value = enemyCurrentHP / enemyMaxHP; //플레이어의 HP를 매 프레임마다 수정
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
        agent.isStopped = true; // 죽을때 Stop 시킴
        if (!isDie)//score+=이 invoke 때문에 누적해서 됨 그걸 방지하기 위함
        {
            ESS.Score++;
            ESS.DieEnemyCount++;
            //오븍젝트 풀에 다시 좀비 넣기. 비활성화
            EnemyAnimator.SetTrigger("Death");
            //Debug.Log("Enemy Die");
            Invoke("AnimewaitObjectPooling", 1f);
        }
        isDie = true;
    }


    void AnimewaitObjectPooling(){
        //Debug.Log("오브젝트 풀로 반납됨.");
        EnemySpawner pool = GameObject.Find("EnemyGenerator").GetComponent<EnemySpawner>();
        gameObject.SetActive(false);
        //enemyCurrentHP = enemyMaxHP; //소용없음.
        isDie = false;
        pool.EnemyObjectPool.Add(gameObject);
    }

    //스켈레톤이 플레이어와 충돌
    public void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == "Player")
        {
            EnemyAttack();
            //Debug.Log("Collision_Attacked");
        }
    }

    //스켈레톤이 총알과 충돌 -> 총알은 Trigger Collider 임
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
