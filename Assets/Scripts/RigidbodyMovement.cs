using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;
using D3xter1922.Scoreboards;
using Proyecto26;

public class RigidbodyMovement : MonoBehaviourPunCallbacks, idamageable
{
    #region variables
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float height = 5f;
    public float heightPadding = 0.05f;
    public LayerMask Ground;
    public float maxGroundAngle = 120;
    public bool debug;
    public float jumpForce = 10f;
    public Camera cam;
    public float SprintFOV = 75f;
    public bool canLook = false;
    [SerializeField] Item[] items;
    int itemIndex, previtemIndex = -1;
    public GameObject itemHolder;
    public Vector3 itemHolderOrigin;
    private float movementCounter, idleCounter;
    private Vector3 targetWeaponBobPosition;
    public Vector2 input;
    float groundAngle;
    Rigidbody rb;
    Vector3 forward;
    Vector3 right;
    Vector3 resultantDir;
    RaycastHit hitInfo;
    [SerializeField]
    bool grounded;
    public bool isSprinting = false;
    [SerializeField]
    public float HP;
    const float MaxHealth = 100f;

    PlayerManager playermanager;
    public PhotonView PV;
    Animator anim;
    public Canvas canvas;
    public TMP_Text ammo;
    public int[] Ammo = new int[2];
    public int[] TotalAmmo = new int[2];

    bool isShooting = false;
    bool _canShoot;

    public float RifleFireRate;

    public Image muzzleFlashImage;
    public Sprite[] flashes;
    [SerializeField]
    Scoreboard scoreBoard;
    public AimDownSights[] ads_scripts;
    [SerializeField] Button leaveLobbyButton;
    [SerializeField] CanvasGroup pauseMenuCG;
    bool isPauseMenuOpen = false;
    public float pistolReloadSpeed, rifleReloadSpeed;
    public Image loadingBar;
    public bool isReloading = false;
    public AudioSource pistolReloadSound;
    public AudioSource rifleReloadSound;
    public Image KillConfirmImage;
    [SerializeField] Color backgroundColorKillfeedForKiller;
    #endregion
    void EquipItem(int _index)
    {

        if (_index == previtemIndex)
            return;

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        if (previtemIndex != -1)
        {
            items[previtemIndex].itemGameObject.SetActive(false);
        }
        previtemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
    void OnShot(ItemInfo info)
    {

    }

    void Awake()
    {
        //Ammo[0] = 12;
        //Ammo[1] = 31;
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        _canShoot = true;
        isShooting = false;
        playermanager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

    }
    private void Start()
    {
        
        HP = MaxHealth;
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(canvas);
        }
        else
        {
            canLook = true;
            EquipItem(0);
        }
        itemHolderOrigin = itemHolder.transform.localPosition;
        KillConfirmImage.fillOrigin = 0;
        KillConfirmImage.fillAmount = 0;
        loadingBar.fillAmount = 0;
    }

    void GetInput()
    {
        input.y = Input.GetAxis("Horizontal");
        input.x = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if (cam != null)
        {
            if (isSprinting && cam.fieldOfView < SprintFOV)
            {
                cam.fieldOfView += 2f;
                return;
            }
            if (!isSprinting && cam.fieldOfView > 60f)
            {
                cam.fieldOfView -= 2f;
                return;
            }
        }

    }
    public GameObject scoreboardUI;
    IEnumerator PlayRecoilAnim(float speed, int magSize, int index)
    {
        isReloading = true;
        if (index == 0)
        {
            pistolReloadSound.Play();
        }else if(index==1)
        {
            rifleReloadSound.Play();
        }
        for (float Current = 0; Current < 100; Current += speed * Time.deltaTime)
        {
            yield return new WaitForSeconds(0.01f);
            loadingBar.fillAmount = Current / 100;
        }
        loadingBar.fillAmount = 0;
        Ammo[index] = magSize;
        isReloading = false;
    }
    void Update()
    {
        if (!PV.IsMine)
            return;
        if (Input.GetKeyDown(KeyCode.R) && !isReloading) 
        {
            if (items[itemIndex].itemInfo.itemName == "Pistol" && TotalAmmo[0] > 0 && Ammo[0] != 12)
            {
                TotalAmmo[0] -= (12 - Ammo[0]);
                if (TotalAmmo[0] + Ammo[0] >= 12)
                {
                    //Ammo[0] = 12;  //add anim later
                    StartCoroutine(PlayRecoilAnim(pistolReloadSpeed, 12, 0));
                }
                else
                    Ammo[0] = TotalAmmo[0] + (12 - Ammo[0]) + Ammo[0];
                if (TotalAmmo[0] < 0)
                {
                    TotalAmmo[0] = 0;
                }
            }
            if (items[itemIndex].itemInfo.itemName == "Rifle" && Ammo[1] != 31)
            {
                TotalAmmo[1] -= (31 - Ammo[1]);
                if (TotalAmmo[1] + Ammo[1] >= 31)
                {
                    //Ammo[1] = 31;  //add anim later
                    StartCoroutine(PlayRecoilAnim(rifleReloadSpeed, 31, 1));
                }
                else
                    Ammo[1] = TotalAmmo[1] + (31 - Ammo[1]) + Ammo[1];
                if (TotalAmmo[1] < 0)
                {
                    TotalAmmo[1] = 0;
                }
            }
        }

        ammo.text = Ammo[itemIndex].ToString("#.00") + "/" + TotalAmmo[itemIndex].ToString("#.00");
        if (itemIndex == 0 && Input.GetMouseButtonDown(0) && Ammo[0] > 0 && !isReloading)
        {
            Ammo[0]--;
            items[0].Use();
            StartCoroutine(MuzzleFlash());
        }
        else if (itemIndex == 1 && Input.GetMouseButton(0) && Ammo[1] > 0 && _canShoot && !isReloading)
        {
            isShooting = true;
            _canShoot = false;
            Ammo[1]--;
            items[1].Use();
            StartCoroutine(ShootAutoGun());
        }
        else if (Input.GetKeyDown(KeyCode.R) && !isReloading) 
        {
            if (items[itemIndex].itemInfo.itemName == "Pistol" && TotalAmmo[0] > 0 && Ammo[0] != 12) 
            {
                TotalAmmo[0] -= (12 - Ammo[0]);
                if (TotalAmmo[0] + Ammo[0] >= 12)
                {
                    //Ammo[0] = 12;  //add anim later
                    StartCoroutine(PlayRecoilAnim(pistolReloadSpeed, 12, 0));
                }
                else
                    Ammo[0] = TotalAmmo[0] + (12 - Ammo[0]) + Ammo[0];
                if (TotalAmmo[0] < 0)
                {
                    TotalAmmo[0] = 0;
                }
            }
            if (items[itemIndex].itemInfo.itemName == "Rifle" && Ammo[1] != 31)
            {
                TotalAmmo[1] -= (31 - Ammo[1]);
                if (TotalAmmo[1] + Ammo[1] >= 31)
                {
                    //Ammo[1] = 31;  //add anim later
                    StartCoroutine(PlayRecoilAnim(rifleReloadSpeed, 31, 1));
                }
                else
                    Ammo[1] = TotalAmmo[1] + (31 - Ammo[1]) + Ammo[1];
                if (TotalAmmo[1] < 0)
                {
                    TotalAmmo[1] = 0;
                }
            }
        }


        resultantDir = Vector3.zero;
        GetInput();
        CheckGround();
        CalculateForward();
        CalculateRight();
        CalculateGroundAngle();
        //DrawDebugLines();

        if (Input.GetKeyDown(KeyCode.Space) && grounded) Jump();

        resultantDir = forward * input.x + right * input.y;

        //sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift) && input.x > 0 && !isReloading) 
        {
            isSprinting = true;

        }
        if ((Input.GetKeyUp(KeyCode.LeftShift) || input.x < 1) && isSprinting)
        {
            isSprinting = false;
        }

        else
        {

            rb.velocity = new Vector3(resultantDir.x * walkSpeed, rb.velocity.y, resultantDir.z * walkSpeed);
        }
        if (isSprinting) Sprint();

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        if (transform.position.y < -100f)
            DiePls();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
        if (!ads_scripts[itemIndex].isADS)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (itemIndex >= items.Length - 1)
                {
                    EquipItem(0);
                }
                else
                {
                    EquipItem(itemIndex + 1);
                }
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (itemIndex <= 0)
                {
                    EquipItem(items.Length - 1);
                }
                else
                {
                    EquipItem(itemIndex - 1);
                }
            }
        }


        //headbob
        if (!ads_scripts[itemIndex].isADS)
        {
            if ((input.x == 0 && input.y == 0) || !grounded)
            {
                HeadBob(idleCounter, 0.015f, 0.015f);
                idleCounter += Time.deltaTime;
                itemHolder.transform.localPosition = Vector3.Lerp(itemHolder.transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
            }
            else if (!isSprinting)
            {
                HeadBob(movementCounter, 0.035f, 0.035f);
                movementCounter += Time.deltaTime * 3f;
                itemHolder.transform.localPosition = Vector3.Lerp(itemHolder.transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else
            {
                HeadBob(movementCounter, 0.1f, 0.06f);
                movementCounter += Time.deltaTime * 7f;
                itemHolder.transform.localPosition = Vector3.Lerp(itemHolder.transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            }
        }
        /*if(Input.GetKey(KeyCode.Escape)&&!isPauseMenuOpen)
        {
            pauseMenuCG.alpha = 1;
            pauseMenuCG.blocksRaycasts = true;
            pauseMenuCG.interactable = true;

        }
        if(Input.GetKey(KeyCode.Escape)&&isPauseMenuOpen)
        {
            pauseMenuCG.alpha = 1;
            pauseMenuCG.blocksRaycasts = false;
            pauseMenuCG.interactable = false;
        }
        leaveLobbyButton.onClick.AddListener(LeaveLobby);*/
        if(Input.GetKeyDown(KeyCode.Y))
        {
            KillConfirmAnimFunc();
        }
        /*
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            KillFeed.instance.AddNewKillListingWithHowImage(PhotonNetwork.LocalPlayer.NickName, "noob", itemIndex, Color.red);
        }*/
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            PV.RPC("RPC_Kill",RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, "noob");
        }

    }
    void KillConfirmAnimFunc()
    {
        StartCoroutine(KillConfirmedAnim());
    }
    void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
    }
    IEnumerator ShootAutoGun()
    {
        StartCoroutine(MuzzleFlash());
        yield return new WaitForSeconds(60 / RifleFireRate);
        _canShoot = true;
    }
    IEnumerator MuzzleFlash()
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
    }
    void Sprint()
    {
        rb.velocity = new Vector3(resultantDir.x * runSpeed, rb.velocity.y, resultantDir.z * runSpeed);
    }
    void Slide()
    {

    }
    void Jump()
    {
        rb.velocity += Vector3.up * jumpForce;
    }

    void CalculateForward()
    {
        if (!grounded)
        {
            forward = transform.forward;
            return;
        }
        forward = Vector3.Cross(transform.right, hitInfo.normal);
    }
    void CalculateRight()
    {
        if (!grounded)
        {
            right = transform.right;
            return;
        }
        right = Vector3.Cross(transform.forward, -hitInfo.normal);
    }
    void CalculateGroundAngle()
    {
        if (!grounded)
        {
            groundAngle = 90;
            return;
        }
        groundAngle = Vector3.Angle(hitInfo.normal, -transform.forward);
    }
    void CheckGround()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hitInfo, height + heightPadding, Ground))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }


    void DrawDebugLines()
    {
        if (!debug)
        {
            return;
        }
        Debug.DrawLine(transform.position, transform.position + forward * height * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position + right * height * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position + resultantDir * height * 2, Color.black);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * height, Color.green);
    }
    [PunRPC]
    void RPC_Update_Scoreboard()
    {
        StartCoroutine(scoreBoard.GetSavedScores());
    }
    IEnumerator KillConfirmedAnim()
    {
        Debug.Log("animKIll");
        KillConfirmImage.fillAmount = 0f;
        for (int i = 0; i < 10; i++)
        {
            if (KillConfirmImage.fillAmount + 0.1f >= 1)
            {
                KillConfirmImage.fillAmount = 1;
                break;
            }
            KillConfirmImage.fillAmount += 0.1f;
            yield return new WaitForSeconds(0.035f);
        }
        /*while (KillConfirmImage.fillAmount < 1)
        {
            if (KillConfirmImage.fillAmount + 0.1f >= 1) 
            {
                KillConfirmImage.fillAmount = 1;
                break;
            }
            KillConfirmImage.fillAmount += 0.1f;
            yield return new WaitForSeconds(0.035f);
        }*/
        yield return new WaitForSeconds(0.1f);
        KillConfirmImage.fillAmount = 1f;
        KillConfirmImage.fillOrigin = 1;
        /*while (KillConfirmImage.fillAmount >0)
        {
            KillConfirmImage.fillAmount -= 0.1f;
            yield return new WaitForSeconds(0.035f);
        }*/
        for (int i = 0; i < 10; i++)
        {
            if (KillConfirmImage.fillAmount - 0.1f <=0)
            {
                KillConfirmImage.fillAmount = 0;
                break;
            }
            KillConfirmImage.fillAmount -= 0.1f;
            yield return new WaitForSeconds(0.035f);
        }
        KillConfirmImage.fillAmount = 0f;
        KillConfirmImage.fillOrigin = 0;
    }
    [PunRPC]
    void RPC_ShowKillfeed(string killerName, string KilledName, int itemIndex)
    {
        Debug.Log("killfeed");
        KillFeed.instance.AddNewKillListingWithHowImage(killerName, KilledName, itemIndex, Color.white);
    }
    [PunRPC]
    void RPC_Kill(int Killerid, string KilledName)
    {
        
        if (PhotonNetwork.LocalPlayer.ActorNumber == Killerid)
        {
            
            PV.RPC("RPC_ShowKillfeed", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, KilledName, itemIndex);
            //KillConfirmImage.fillOrigin = 0;
            //KillConfirmImage.fillAmount = 0;
            KillConfirmAnimFunc();
            string killerName;
            killerName = PhotonNetwork.LocalPlayer.NickName;
            //KillFeed.instance.AddNewKillListingWithHowImage(killerName, KilledName, itemIndex, Color.white);

            StartCoroutine(scoreBoard.ChangeUI((int)Killerid, 1, 0));
            StartCoroutine(scoreBoard.GetSavedScores());
            
            scoreBoard.RPC_SyncUI();
            //KillFeed.instance.AddNewKillListingWithHowImage(killerName, KilledName, itemIndex, backgroundColorKillfeedForKiller);
                   
        }
    }
    public void TakeDamage(float damage, float id)
    {
        //HP -= damage;
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, id);
    }
    [PunRPC]
    void RPC_TakeDamage(float damage, float id)
    {
        if (!PV.IsMine)
            return;

        HP -= damage;
        if (HP <= 0)
        {
            PV.RPC("RPC_Kill", RpcTarget.All, (int)id, PhotonNetwork.LocalPlayer.NickName);
            DiePls();
        }
    }

    void DiePls()
    {
        StartCoroutine(scoreBoard.ChangeUI(PhotonNetwork.LocalPlayer.ActorNumber, 0, 1));

        scoreBoard.RPC_SyncUI();
        
        playermanager.DiePls();
    }
    /*void DiePls()
    {
        //updateUI
        StartCoroutine(scoreBoard.ChangeUI(PhotonNetwork.LocalPlayer.ActorNumber, 0, 1));

        scoreBoard.RPC_SyncUI();
        playermanager.DiePls();
        //PhotonNetwork.CurrentRoom.Name;
    }*/

    void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
    {
        targetWeaponBobPosition = itemHolderOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0f);
    }
}

