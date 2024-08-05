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
    
    //����
    private EnemySpawner ESS;

    [Header("Aim")]
    [SerializeField]
    private CinemachineVirtualCamera aimCam;
    [SerializeField]
    private GameObject aimImage;
    [SerializeField]
    private GameObject aimObj; // ���� �ٶ󺸴� ��ü�� �������� ���� ������.
    [SerializeField]
    private float aimObjDis = 5f; //�� �Ÿ���- 5f ������ Ray�� �˻��ϰԲ�
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
            //�״� ����� �����ϰ� ��ŵ�Ѵ�. 
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
        //���� ķ ������Ʈ�� Ȱ��ȭ, ũ�ν���� ���� �̹����� Ȱ��ȭ Ȥ�� ��Ȱ��ȭ
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
        //Quaternion rot = Quaternion.Euler(DirAim); // ���Ϸ����� ���͸� ���ʹϾ� ȸ�������� ������

        //�Ѿ��� �����Ҷ�
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(weaponFlashFX, GunFront.transform);//�ѿ��� ������ ��ƼŬ
            GameObject GunnerBullet = Instantiate(GunnerBulletPrefab, GunFront.transform.position, Quaternion.LookRotation(AimDirection, Vector3.up));
            playerAudio.PlayOneShot(GunFireAudio);
            BulletCount--;
        }
    }

    Vector3 AimPoint()
    {
        Vector3 targetPosition = Vector3.zero;

        //ī�޶� �ٶ󺸴� ��Ʈ ����� �˾�ä�� ����
        Transform camTransform = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer)){
            //Debug.Log("Name : " + hit.transform.gameObject.name);
            targetPosition = hit.point; // Ÿ�� �������� �浹������ hit.point���� ����
            aimObj.transform.position = hit.point;
        }
        else{
            targetPosition = camTransform.position + camTransform.forward * aimObjDis;
            aimObj.transform.position = camTransform.position + camTransform.forward * aimObjDis;
        }

        Vector3 targetAim = targetPosition;
        //ĳ������ y���� �������� y���� �����ֱ����� ������.
        targetAim.y = transform.position.y;
        //��ġ���� ������ ��Ƶ� ���� 3 ���� ��������
        Vector3 aimDir = (targetAim - transform.position).normalized;

        //ĳ���Ͱ� ��������� �� �� �ְ� �ϰ�, �ε巴�� �ϱ����� Lerp�� �ۼ���.
        transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime);

        //SetRigWeight(1);
        return targetPosition;
    }

    //�÷��̾ ���̷���� �浹�Ͽ�����, HP�� ����.
    public void OnCollisionEnter(Collision collision){
        //bool isHurt = false;
        if(collision.gameObject.tag == "Enemy")
        {
            HPUpdate();
            playerAudio.PlayOneShot(HurtSound);
        }
    }

}
