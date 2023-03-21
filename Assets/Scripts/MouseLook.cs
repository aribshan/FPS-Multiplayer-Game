using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    public GameObject arm;
    public GameObject startingPoint;
    RigidbodyMovement rbmovement;
    SingleShotGun _singleshotgun;
    public float xRecoil = 0;
    public float yRecoil = 0;
    public float recoilSpeed, dampeningSpeed;
    float raw_mouseX, mouseX;
    [SerializeField]
    float raw_mouseY, mouseY;
    public float maxYRecoil;
    public float t = 0f;
    float returnYrecoil = 0f;
    float returnRecoilMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        rbmovement = FindObjectOfType<RigidbodyMovement>();
        //_singleshotgun = GetComponent<SingleShotGun>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (rbmovement.canLook)
        { 
            mouseX = xRecoil + Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            mouseY = yRecoil + Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime + returnYrecoil;
            xRecoil -= recoilSpeed * Time.deltaTime;
            if (yRecoil > 0)
                yRecoil -= recoilSpeed * Time.deltaTime;
            if (xRecoil < 0) 
            {
                xRecoil = 0;
            }
            if (yRecoil < 0 && yRecoil > -maxYRecoil/returnRecoilMultiplier)   
            {
                yRecoil -= recoilSpeed * Time.deltaTime / 25;
                //returnYrecoil -= recoilSpeed * Time.deltaTime / 20;
            }
            if (yRecoil <= -maxYRecoil/returnRecoilMultiplier) 
            {
                yRecoil = 0;
            }
            /*if(returnYrecoil <= -0.1)
            {
                returnYrecoil = 0;
            }*/

            if (xRecoil == 0)
            {
                raw_mouseY = mouseY;
            }
            //Debug.Log("_________"+mouseY);
            
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 75f);
            arm.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
    public void AddRecoil(float up, float side, float returnRecoilSens)
    {
        returnRecoilMultiplier = returnRecoilSens;
        if (maxYRecoil < yRecoil) 
        {
            maxYRecoil = yRecoil;
        }
        if (yRecoil < 0)
            yRecoil = 0;
        yRecoil += up;
        xRecoil += side;
        maxYRecoil = up;
    }
    
}
