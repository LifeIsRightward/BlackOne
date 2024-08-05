using Cinemachine;
using StarterAssets;
using Unity.VisualScripting;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static int PlayerBulletPower;
    public static float CurrentHP;
    public static float HPMAX;
    public static int MaxBullet;
    public int BulletCount;

    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private Animator anim;
    private AudioSource playerAudio;

    public Image HPBar;
    public bool isPlayerDead = false;
    public UIManager UIManager;
    public GameObject ReloadingMessage;
    public AudioClip NoneBulletSound;
    public AudioClip GunFireAudio;
    public AudioClip ReloadingSound;
    public AudioClip HurtSound;
    public GameObject GunnerBulletPrefab;
    
    //수정
    private EnemySpawner ESS;

    [Header("Aim")]
    [SerializeField]
    private CinemachineVirtualCamera aimCam;
    [SerializeField]
    private GameObject aimImage;
    [SerializeField]
    private GameObject aimObj; // 내가 바라보는 물체를 가져오기 위한 변수임.
    [SerializeField]
    private float aimObjDis = 5f; //그 거리가- 5f 까지만 Ray가 검사하게끔
    [SerializeField]
    private LayerMask targetLayer;

    [Header("IK")]
    [SerializeField]
    private Rig handRig;
    [SerializeField]
    private Rig aimRig;

    [Header("Weapon FX")]
    [SerializeField]
    private GameObject weaponFlashFX;


    // Start is called before the first frame update
    void Start(){
        PlayerBulletPower = 10;
        MaxBullet = 30;
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        InitHP();
        BulletInit();
        ESS = GameObject.Find("EnemyGenerator").GetComponent<EnemySpawner>();

    }
    // Update is called once per frame
    void Update(){
        AimCheck();
        PlayerIsDead();
        HPBar.fillAmount = CurrentHP / HPMAX;
    }
    
    void InitHP(){
        CurrentHP = 100f;
        HPMAX = 100f;
        HPBar.type = Image.Type.Filled;
        HPBar.fillMethod = Image.FillMethod.Horizontal;
        HPBar.fillOrigin = (int)Image.OriginHorizontal.Right;
    }

    void BulletInit()
    {
        PlayerBulletPower = 10;
        BulletCount = MaxBullet;
    }

    void HPUpdate(){
        CurrentHP -= ESS.WaveEnemyAttackStat;
        HPBar.fillAmount = CurrentHP / HPMAX;
        Debug.Log(CurrentHP);
    }
    
    public void PlayerIsDead(){
        if (CurrentHP <= 0){
            Debug.Log("Player Dead");
            isPlayerDead = true;
            //죽는 모션은 과감하게 스킵한다. 
        }  
    }

    private void AimCheck()
    {
        if (input.reroad){
            input.reroad = false;
            //Debug.Log("In reroad");
            if (controller.isReroad){
                //Debug.Log("No excute");
                return;
            }

            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("Reroad");
            //Debug.Log("After Reroad");
            ReloadingMessage.SetActive(false);
            playerAudio.PlayOneShot(ReloadingSound);
            controller.isReroad = true;
        }

        if (controller.isReroad){
            return;
        }

        if (input.aim){
            AimControll(true);
            anim.SetLayerWeight(1, 1);
            AimPoint();
            SetRigWeight(1);

            if (input.shoot)
            {
                if (BulletCount <= 0)
                {
                    anim.SetBool("Shoot", true);
                    ReloadingMessage.SetActive(true);
                    playerAudio.PlayOneShot(NoneBulletSound);
                    //Reroad();
                }
                else
                {
                    anim.SetBool("Shoot", true);
                    PlayerFire();
                }
            }else
            {
                anim.SetBool("Shoot", false);
            }
        }
        else{
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);
            anim.SetBool("Shoot", false);
        }
    }

    private void AimControll(bool isCheck){
        //에임 캠 오브젝트를 활성화, 크로스헤어 에임 이미지를 활성화 혹은 비활성화
        aimCam.gameObject.SetActive(isCheck);
        aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    public void Reroad(){
        //Debug.Log("Reroad");
        controller.isReroad = false;
        SetRigWeight(1);
        anim.SetLayerWeight(1, 0);
        BulletCount = MaxBullet;
    }

    private void SetRigWeight(float weight){
        aimRig.weight = weight;
        handRig.weight = weight;
    }

    void PlayerFire(){
        GameObject GunFront = GameObject.Find("GunFrontPoint");
        Vector3 DirAim = AimPoint();
        Vector3 AimDirection = (DirAim - GunFront.transform.position).normalized;

        //GameObject GunnerBullet = Instantiate(GunnerBulletPrefab, GunFront.transform.position, Quaternion.LookRotation(AimDirection, Vector3.up));

        //Debug.Log(DirAim.ToString());
        //Quaternion rot = Quaternion.Euler(DirAim); // 오일러각인 벡터를 쿼터니언 회전각으로 컨버팅

        //총알을 발포할때
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(weaponFlashFX, GunFront.transform);//총에서 나오는 파티클
            GameObject GunnerBullet = Instantiate(GunnerBulletPrefab, GunFront.transform.position, Quaternion.LookRotation(AimDirection, Vector3.up));
            playerAudio.PlayOneShot(GunFireAudio);
            BulletCount--;
        }
    }

    Vector3 AimPoint()
    {
        Vector3 targetPosition = Vector3.zero;

        //카메라가 바라보는 히트 대상을 알아채기 위함
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer)){
            //Debug.Log("Name : " + hit.transform.gameObject.name);
            targetPosition = hit.point; // 타켓 포지션의 충돌지점인 hit.point값을 대입
            aimObj.transform.position = hit.point;
        }
        else{
            targetPosition = camTransform.position + camTransform.forward * aimObjDis;
            aimObj.transform.position = camTransform.position + camTransform.forward * aimObjDis;
        }

        Vector3 targetAim = targetPosition;
        //캐릭터의 y값과 조준점의 y값을 맞춰주기위해 대입함.
        targetAim.y = transform.position.y;
        //위치값의 방향을 담아둘 벡터 3 변수 단위벡터
        Vector3 aimDir = (targetAim - transform.position).normalized;

        //캐릭터가 전방방향을 볼 수 있게 하고, 부드럽게 하기위해 Lerp를 작성함.
        transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime);

        //SetRigWeight(1);
        return targetPosition;
    }

    //플레이어가 스켈레톤과 충돌하였을때, HP가 깎임.
    public void OnCollisionEnter(Collision collision){
        //bool isHurt = false;
        if(collision.gameObject.tag == "Enemy")
        {
            HPUpdate();
            playerAudio.PlayOneShot(HurtSound);
        }
    }

}
