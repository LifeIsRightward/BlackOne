//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletSpeed = 30f;
    Rigidbody BulletRB;
    // Start is called before the first frame update
    void Start(){
        BulletRB = GetComponent<Rigidbody>();
        BulletRB.velocity = transform.forward*BulletSpeed;

        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other){
        if (other.tag == "Enemy"){
            //Debug.Log("HIT!!");
            Destroy(gameObject); // ÃÑ¾ËÀÌ ºÎ½¤Áü
        }
    }
}
