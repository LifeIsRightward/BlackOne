//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class LookUI : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    void Start(){
        if(cam == null){
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update(){
        if(cam != null){
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}
