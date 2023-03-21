using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour
{
    bool isActive = false;
    public CanvasGroup cg;
    public GraphicRaycaster gr;
    // Start is called before the first frame update
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        gr = GetComponent<GraphicRaycaster>();
        gr.enabled = false;
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Escape) && !isActive)
        {
            isActive = true;
            cg.alpha = 1;
            gr.enabled = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && isActive)
        {
            isActive = false;
            cg.alpha = 0;
            gr.enabled = false;
            return;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.JoinLobby();
    }
}
