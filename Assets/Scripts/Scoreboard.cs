using System.IO;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Proyecto26;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace D3xter1922.Scoreboards
{
    
    public class Scoreboard : MonoBehaviour

    {
        [SerializeField] private int maxScoreboardEntries = 5;
        [SerializeField] private Transform highscoreholderTransform = null;
        [SerializeField] private GameObject scoreboardEntryObject = null;

        [Header("test")]
        [SerializeField] ScoreboardEntryData testEntryData = new ScoreboardEntryData();
        CanvasGroup cg;
        [SerializeField] PhotonView PV;
        public ScoreboardSaveData scoreboardSaves = new ScoreboardSaveData();
        public int gameOverKills = 5;
        [SerializeField] Canvas gameOverCG;
        [SerializeField] TextMeshProUGUI P1Text,subText;
        [SerializeField] GameObject hideCanvas1, hideCanvas2;
        [SerializeField] Menu loadingMenu, titleMenu;
        private void Start()
        {
            
            cg = GetComponentInChildren<CanvasGroup>();
            PV = GetComponent<PhotonView>();
            StartCoroutine(GetSavedScores());
            //StartCoroutine(ChangeUI(PhotonNetwork.LocalPlayer.ActorNumber, 0, 0));
            gameOverCG.GetComponent<GraphicRaycaster>().enabled = false;
            if (GameObject.Find("PlayerManager(Clone)") == null)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            }
        }

        private void Update()
        {
            GameOver();
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                StartCoroutine(GetSavedScores());
                cg.alpha = 1;
            }
            if (Input.GetKeyUp(KeyCode.BackQuote))
            {
                cg.alpha = 0;
            }
            /*if(Input.GetKeyDown(KeyCode.P))
            {

                StartCoroutine(ChangeUI(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1));


            }*/
            /*if(Input.GetKeyDown(KeyCode.O))
            {
                StartCoroutine(GetSavedScores());
            }*/
        }
        /*private IEnumerator GetAndChangeUI(int ID, int killIncrement, int deathIncrement)
        {
            StartCoroutine(GetSavedScores());
            yield return new WaitForSeconds(1f);
            StartCoroutine(ChangeUI(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1));
        }*/
        public void UpdateUI()
        {
            
            foreach(Transform child in highscoreholderTransform)
            {
                
                Destroy(child.gameObject);
            }
            foreach(ScoreboardEntryData highscore in scoreboardSaves.scores)
            {
                //Debug.Log("updating entry");
                Instantiate(scoreboardEntryObject, highscoreholderTransform).GetComponent<ScoreboardEntryUI>().Initialize(highscore);
            }
        }
        
        public IEnumerator ChangeUI(int ID, int kilIncrement, int deathIncrement)
        {
            ScoreboardEntryData SED;
            
            //Debug.Log("found guy");

            RestClient.Get<ScoreboardEntryData>(url: $"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{PhotonNetwork.CurrentRoom.Name}/users/{ID}.json").Then(response =>
            {
                SED.ID = ID;
                SED.entryName = response.entryName;
                SED.entryKills = response.entryKills + kilIncrement;
                SED.entryDeaths = response.entryDeaths + deathIncrement;
                RestClient.Put(url: $"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{PhotonNetwork.CurrentRoom.Name}/users/{ID}.json", SED);
            });
            yield return new WaitForSeconds(0.2f);
            PV.RPC("RPC_SyncUI", RpcTarget.All);

            StartCoroutine(GetSavedScores());




        }
        [PunRPC]
        public void RPC_SyncUI()
        {
            StartCoroutine(GetSavedScores());
        }
        public IEnumerator GetSavedScores()
        {
            Debug.Log("gettingSavedScored");
            ScoreboardSaveData ssd = new ScoreboardSaveData();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                RestClient.Get<ScoreboardEntryData>(url: $"https://cgl-game-default-rtdb.asia-southeast1.firebasedatabase.app/{PhotonNetwork.CurrentRoom.Name}/users/{PhotonNetwork.PlayerList[i].ActorNumber}.json").Then(response =>
                {
                    ssd.scores.Add(response);
                });
            }
            yield return new WaitForSeconds(0.2f);
            scoreboardSaves = ssd;
            UpdateUI();
        }
        [PunRPC]
        void EndGame(int _winnerID, string _winnerName, int _winnerKills, int _winnerDeaths)
        {
            P1Text.text = "1. " + _winnerName;
            subText.text = $"Kills: {_winnerKills}     Deaths: {_winnerDeaths}     K/D: {((_winnerDeaths == 0) ? _winnerKills : (float)_winnerKills / _winnerDeaths)}";
            //hideCanvas1.SetActive(false);
            //hideCanvas2.SetActive(false);
            gameOverCG.GetComponent<CanvasGroup>().alpha = 1;
            gameOverCG.GetComponent<GraphicRaycaster>().enabled = true;

        }
        public void GoToMainMenu()
        {
            PhotonNetwork.LeaveRoom();
            //loadingMenu.Close();
            //titleMenu.Open();
            //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            PhotonNetwork.LoadLevel(0);
            PhotonNetwork.JoinLobby();
        }
        void GameOver()
        {
            
            int winnerID;
            foreach(ScoreboardEntryData _entryData in scoreboardSaves.scores)
            {
                if (_entryData.entryKills == gameOverKills)
                {
                    winnerID = _entryData.ID;
                    PV.RPC("EndGame", RpcTarget.All, winnerID, _entryData.entryName, _entryData.entryKills, _entryData.entryDeaths);
                    break;
                }
            }
        }
    }
}


