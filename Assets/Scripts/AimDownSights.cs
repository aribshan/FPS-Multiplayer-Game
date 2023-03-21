using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDownSights : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 hipFirePosition;
    public Vector3 ADSPosition;
    public float ADSSpeed;
    public Camera cam;
    public RigidbodyMovement rbMove;
    public bool isADS = false;
    AudioSource _audioSource;
    PhotonView pv;
    AudioClip aimSound;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
    }
    [PunRPC]
    void RPC_syncAimSound(Vector3 position)
    {
        AudioSource audioRPC = gameObject.AddComponent<AudioSource>();
        audioRPC.clip = aimSound;
        audioRPC.spatialBlend = 1;
        AudioSource.PlayClipAtPoint(audioRPC.clip, position);

    }
    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            if (rbMove.isSprinting == false)
            {
                if(Input.GetMouseButtonDown(1)||Input.GetMouseButtonUp(1))
                {
                    _audioSource.Play();
                    pv.RPC("RPC_syncAimSound",RpcTarget.Others, transform.position);
                }
                if (Input.GetMouseButton(1))
                {
                    rbMove.itemHolder.transform.localPosition = Vector3.Lerp(rbMove.itemHolder.transform.localPosition, rbMove.itemHolderOrigin, Time.deltaTime * ADSSpeed);
                    transform.localPosition = Vector3.Slerp(transform.localPosition, ADSPosition, ADSSpeed * Time.deltaTime);
                    cam.fieldOfView = Vector3.Slerp(new Vector3(cam.fieldOfView, 0, 0), new Vector3(45, 0, 0), ADSSpeed * Time.deltaTime).x;
                    isADS = true;
                }
                if (!Input.GetMouseButton(1) && cam.fieldOfView <= 59.9)
                {
                    transform.localPosition = Vector3.Slerp(transform.localPosition, hipFirePosition, ADSSpeed * Time.deltaTime);
                    cam.fieldOfView = Vector3.Slerp(new Vector3(cam.fieldOfView, 0, 0), new Vector3(60, 0, 0), ADSSpeed * Time.deltaTime).x;
                    isADS = false;
                }
            }
        }
    }
    public void shakeGun()
    {
        Vector3 shakePos = transform.localPosition + new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(0f, 0.1f), Random.Range(-0.2f, -0.05f));
        transform.localPosition = Vector3.Lerp(transform.localPosition, shakePos, 1 * Time.deltaTime);
        
    }
}
