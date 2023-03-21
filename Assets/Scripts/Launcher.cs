using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Proyecto26;
using D3xter1922.Scoreboards;
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform PlayerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    public string databaseURL = "https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/";
    public string username;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Debug.Log("connecting to master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public void SetUsername(string s)
    {
        if (s == "")
        {
            username = "Player" + Random.Range(0, 1000).ToString("0000");
        }
        else
        {
            Debug.Log("set username");
            username = s;
            PhotonNetwork.NickName = username;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("joined master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("joined lobby");
        /***********/PhotonNetwork.NickName = username;
    }
    
    
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        RestClient.Delete($"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{roomNameInputField.text}.json");
        
        MenuManager.Instance.OpenMenu("loading");

    }
    public override void OnCreatedRoom()
    {
        ScoreboardEntryData scoreboardData;
        scoreboardData.ID = PhotonNetwork.LocalPlayer.ActorNumber;
        scoreboardData.entryName = PhotonNetwork.LocalPlayer.NickName;
        scoreboardData.entryKills = 0;
        scoreboardData.entryDeaths = 0;
        
        
        //RestClient.Post(url: $"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{roomNameInputField.text}/users/{PhotonNetwork.LocalPlayer.ActorNumber}.json", scoreboardData);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        RestClient.Delete($"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{roomNameInputField.text}/users/{PhotonNetwork.LocalPlayer.ActorNumber}.json");
    }
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in PlayerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        ScoreboardEntryData scoreboardData;
        scoreboardData.ID = PhotonNetwork.LocalPlayer.ActorNumber;
        scoreboardData.entryName = PhotonNetwork.LocalPlayer.NickName;
        scoreboardData.entryKills = 0;
        scoreboardData.entryDeaths = 0;
        RestClient.Put(url: $"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{PhotonNetwork.CurrentRoom.Name}/users/{PhotonNetwork.LocalPlayer.ActorNumber}.json", scoreboardData);
        
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "room creation failed" + message;
        MenuManager.Instance.OpenMenu("error");
    }
    

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
        RestClient.Delete($"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{PhotonNetwork.CurrentRoom.Name}/users/{PhotonNetwork.LocalPlayer.ActorNumber}.json");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("leave");
        
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList)
            {
                Debug.Log(roomList[i].Name);
                RestClient.Delete($"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{roomList[i].Name}.json");
                continue;

            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
