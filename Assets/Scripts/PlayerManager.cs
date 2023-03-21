using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    private void Awake()
    {
        Debug.Log("run");
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        Debug.Log("running");
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Debug.Log("instantiated player controller");
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "realPlayer"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void DiePls()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}

