using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SingleShotGun : Gun
{
    [SerializeField]
    Camera cam;
    public float CalculatedvRecoil;
    public float CalculatedhRecoil;
    bool isShooting = false;
    int i = 30;
    [SerializeField]
    GameObject CamHolder;
    public MouseLook _mouselook;
    public GameObject g;
    public AimDownSights ads;
    public GameObject[] BulletHolePrefabs;
    Vector3 originPosition;
    public RigidbodyMovement rbmove;
    public AudioSource ShootSound;
    [SerializeField]
    ParticleSystem muzzleParticleSystem;
    public AudioClip remoteAudioclip;
    [SerializeField] PhotonView pv;
    private void Start()
    {
        _mouselook = g.GetComponent<MouseLook>();
        originPosition = transform.localPosition;
        ShootSound = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
    }
    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originPosition, Time.deltaTime * 4f);
    }
    public override void Use()
    {
        Shoot();
        
    }
    [PunRPC]
    public void RPC_soundSync(string audioname, Vector3 position)
    {

        AudioSource audioRPC = gameObject.AddComponent<AudioSource>();
        audioRPC.clip = remoteAudioclip; 
        audioRPC.spatialBlend = 1; 
        audioRPC.minDistance = 25; 
        audioRPC.maxDistance = 100; 
        //audioRPC.Play();
        Debug.Log("play sound: " + gameObject.GetComponent<PhotonView>().ViewID.ToString());
        AudioSource.PlayClipAtPoint(audioRPC.clip, position);
    }
    void Shoot()
    {
        muzzleParticleSystem.Play();
        ShootSound.Play();
        pv.RPC("RPC_soundSync", RpcTarget.Others, "Pistol_01_Fire_04_SFX", transform.position);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (!ads.isADS)
            ray.direction += new Vector3(Random.Range(-((GunInfo)itemInfo).bulletSpread, ((GunInfo)itemInfo).bulletSpread), Random.Range(-((GunInfo)itemInfo).bulletSpread, ((GunInfo)itemInfo).bulletSpread), 0);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            
            hit.collider.gameObject.GetComponent<idamageable>()?.TakeDamage(((GunInfo)itemInfo).damage, PhotonNetwork.LocalPlayer.ActorNumber);
            
            //bullet hole
            int i = (int)Random.Range(0, 3);
            GameObject t_newHole = Instantiate(BulletHolePrefabs[i], hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            t_newHole.transform.LookAt(hit.point + hit.normal);
            Destroy(t_newHole, 120f);
        }
        CalculatedvRecoil = ((GunInfo)itemInfo).vRecoil + Random.Range(-((GunInfo)itemInfo).vRecoilRandomness, ((GunInfo)itemInfo).vRecoilRandomness);
        CalculatedhRecoil = Random.Range(-((GunInfo)itemInfo).hRecoilRandomness, ((GunInfo)itemInfo).hRecoilRandomness);
        if (!ads.isADS)
            _mouselook.AddRecoil(CalculatedvRecoil / 15, CalculatedhRecoil / 15, ((GunInfo)itemInfo).returnRecoilSens);
        else
            _mouselook.AddRecoil(CalculatedvRecoil / 10, CalculatedhRecoil / 10, ((GunInfo)itemInfo).returnRecoilSens);

        //gun fx
        if (!ads.isADS)
            transform.Rotate(-((GunInfo)itemInfo).vRecoil / 2, 0, 0);
        transform.position -= transform.forward * ((GunInfo)itemInfo).weaponKick;
    }
    
    


}
