using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public static PhotonInit instance;

    public InputField playerInput;
    public Button chattingBtn;

    public bool isGameStart = false;
    public bool isLoggIn = false;
    public bool isReady = false;
    string playerName = "";
    string connectionState = "";
    public string chatMessage;
    Text chatText;
    ScrollRect scroll_rext = null;

    PhotonView pv;

    Text connectionInfoText;

    [Header("LobbyCanvas")] 
    public GameObject LobbyCanvas;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public GameObject MakeRoomPanel;
    public InputField RoomInput;
    public InputField RoomPwInput;
    public Toggle PwToggle;
    public GameObject PwPanel;
    public GameObject pwErrorLog;
    public GameObject PwConfirmBtn;
    public GameObject PwPanelCloseBtn;
    public InputField PwCheckIF;
    public bool LockState = false;
    public string privateroom;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button nextBtn;
    public Button CreateRoomBtn;
    public int hashtablecount;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple, roomnumber;

    void Awake()
    {
        PhotonNetwork.GameVersion = "MyFps 1.0";
        PhotonNetwork.ConnectUsingSettings();

        if (GameObject.Find("ChatText") != null)
        {
            chatText = GameObject.Find("ChatText").GetComponent<Text>();
        }

        if (GameObject.Find("Scroll View") != null)
        {
            scroll_rext = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        }

        if (GameObject.Find("ConnectionInfoText") != null)
        {
            connectionInfoText = GameObject.Find("ConnectionInfoText").GetComponent<Text>();
        }

        connectionState = "마스터 서버에 접속 중...";


        if (connectionInfoText)
        {
            connectionInfoText.text = connectionState;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("LogIn") == 1)
        {
            isLoggIn = true;
        }

        if(isGameStart == false && SceneManager.GetActiveScene().name == "HyeonSeongScene" && isLoggIn == true)
        {
            Debug.Log("Update : " + isGameStart + ", " + isLoggIn);
            isGameStart = true;
            if(GameObject.Find("ChatText") != null)
            {
                chatText = GameObject.Find("ChatText").GetComponent<Text>();
            }

            if(GameObject.Find("Scroll View") != null)
            {
                scroll_rext = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
            }

            if(GameObject.Find("InputFieldChat") != null)
            {
                playerInput = GameObject.Find("InputFieldChat").GetComponent<InputField>();
            }
            if(GameObject.Find("ChattingButton") != null)
            {
                chattingBtn = GameObject.Find("ChattingButton").GetComponent<Button>();
                chattingBtn.onClick.AddListener(SetPlayerName);
            }
            StartCoroutine(CreatePlayer());
        }
    }

    public static PhotonInit Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType(typeof(PhotonInit)) as PhotonInit;

                if (instance == null)
                {
                    Debug.Log("no singleton obj");
                }
            }

            return instance;
        }
    }

    public void Connect()
    {
        if(PhotonNetwork.IsConnected && isReady)
        {
            connectionState = "룸에 접속...";
            if(connectionInfoText)
            {
                connectionInfoText.text = connectionState;
            }

            LobbyPanel.SetActive(false);
            RoomPanel.SetActive(true);

            PhotonNetwork.JoinLobby();
        }
        else
        {
            connectionState = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도중...";
            if (connectionInfoText)
            {
                connectionInfoText.text = connectionState;
            }
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionState = "No Room";
        if (connectionInfoText)
        {
            connectionInfoText.text = connectionState;
        }
        Debug.Log("No Room");
        //PhotonNetwork.CreateRoom("MyRoom");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        connectionState = "Finish make a room";
        if (connectionInfoText)
        {
            connectionInfoText.text = connectionState;
        }
        Debug.Log("Finish make a room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        connectionState = "Joined room";
        if (connectionInfoText)
        {
            connectionInfoText.text = connectionState;
        }
        Debug.Log("Joined room");
        isLoggIn = true;
        PlayerPrefs.SetInt("LogIn", 1);

        PhotonNetwork.LoadLevel("HyeonSeongScene");
        DontDestroyOnLoad(LobbyCanvas.gameObject);
        LobbyCanvas.SetActive(false);
        //SceneManager.LoadScene("SampleScene");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("LogIn", 0);
    }

    IEnumerator CreatePlayer()
    {
        while(!isGameStart)
        {
            yield return new WaitForSeconds(0.5f);
        }

        GameObject tempPlayer = PhotonNetwork.Instantiate("Player", new Vector3(5, 0, 6), Quaternion.identity, 0);

        //tempPlayer.GetComponent<PlayerCtrl>().SetPlayerName(playerName);
        pv = GetComponent<PhotonView>();

        yield return null;
    }

    private void OnGUI()
    {
        GUILayout.Label(connectionState);
    }

    public void SetPlayerName()
    {
        Debug.Log(playerInput.text + "를 입력 하셨습니다!");

        if (isGameStart == false && isLoggIn == false)
        {
            playerName = playerInput.text;
            playerInput.text = string.Empty;
            Debug.Log("connect 시도!" + isGameStart + ", " + isLoggIn);
            Connect();
        }
        else if (isGameStart == true && isLoggIn == true)
        {
            chatMessage = playerInput.text;
            playerInput.text = string.Empty;
            pv.RPC("ChatInfo", RpcTarget.All, chatMessage);
            
        }
    }

    public void ShowChat(string chat)
    {
        chatText.text += chat + "\n";

        scroll_rext.verticalNormalizedPosition = 0.0f;
    }

    [PunRPC]
    public void ChatInfo(string sChat, PhotonMessageInfo info)
    {
        ShowChat(sChat);
    }

    #region 방 생성 및 접속 관련 메서드
    public void CreateRoomBtnOnClick()
    {
        MakeRoomPanel.SetActive(true);
    }

    public void OKBtnOnClick()
    {
        MakeRoomPanel.SetActive(false);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Game" + Random.Range(0, 100) : RoomInput.text,
            new RoomOptions { MaxPlayers = 4 });
        LobbyPanel.SetActive(false);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        connectionState = "마스터 서버에 접속 중...";
        if(connectionInfoText)
        {
            connectionInfoText.text = connectionState;
        }
        isGameStart = false;
        isLoggIn = false;
        PlayerPrefs.SetInt("LogIn", 0);
    }

    public void CreateNewRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.CustomRoomProperties = new Hashtable()
        {
            { "password", RoomPwInput.text}
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };

        if(PwToggle.isOn)
        {
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Game" + Random.Range(0, 100) : "*" + RoomInput.text,
                roomOptions);
        }
        else
        {
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Game" + Random.Range(0, 100) : RoomInput.text,
                new RoomOptions { MaxPlayers = 4 });
        }

        MakeRoomPanel.SetActive(false);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        myList.Clear();
        Debug.Log("OnRoomListUpdate : " + roomList.Count);
        int roomCount = roomList.Count;
        for(int i = 0; i < roomCount; i++)
        {
            if(!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i]))
                {
                    myList.Add(roomList[i]);
                }
                else
                {
                    myList[myList.IndexOf(roomList[i])] = roomList[i];
                }
            }
            else if(myList.IndexOf(roomList[i]) != -1)
            {
                myList.RemoveAt(myList.IndexOf(roomList[i]));
            }
        }
        MyListRenewal();
    }

    public void MyListClick(int num)
    {
        if(num == -2)
        {
            --currentPage;
            MyListRenewal();
        }
        else if(num == -1)
        {
            ++currentPage;
            MyListRenewal();
        }
        else if(myList[multiple + num].CustomProperties["password"] != null)
        {
            PwPanel.SetActive(true);
        }
        else
        {
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
            MyListRenewal();
        }
    }

    public void RoomPw(int number)
    {
        switch(number)
        {
            case 0:
                roomnumber = 0;
                break;
            case 1:
                roomnumber = 1;
                break;
            case 2:
                roomnumber = 2;
                break;
            case 3:
                roomnumber = 3;
                break;

            default:
                break;
        }
    }

    public void EnterRoomWithPW()
    {
        if((string)myList[multiple + roomnumber].CustomProperties["password"] == PwCheckIF.text)
        {
            PhotonNetwork.JoinRoom(myList[multiple + roomnumber].Name);
            MyListRenewal();
            PwPanel.SetActive(false);
        }
        else
        {
            StartCoroutine("ShowPwWrongMsg");
        }
             
    }

    IEnumerator ShowPwWrongMsg()
    {
        if(!pwErrorLog.activeSelf)
        {
            pwErrorLog.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            pwErrorLog.SetActive(false);
        }
    }

    public void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0)
            ? myList.Count / CellBtn.Length
            : myList.Count / CellBtn.Length + 1;

        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        nextBtn.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * CellBtn.Length;
        for(int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count)
                ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToServer");
        isReady = true;
    }

    public void TogglePw()
    {
        RoomPwInput.gameObject.SetActive(PwToggle.isOn);
    }

    #endregion
}
