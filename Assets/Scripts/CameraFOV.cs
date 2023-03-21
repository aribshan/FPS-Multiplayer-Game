using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            
            anim.SetTrigger("sprinting");
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Debug.Log("stopsprint");
        }
    }
}
