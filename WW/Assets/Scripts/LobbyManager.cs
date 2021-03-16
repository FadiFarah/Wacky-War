using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    [Header("---UI Screens---")]
    public GameObject roomUI;
    public GameObject lobbyUI;
    public GameObject createRoomUI;
    public GameObject JoinRoomUI;

    [Header("---UI Text---")]
    public Text statusText;
    public Text connectingText;
    public Text startBtnText;
    public Text lobbyText;
    

    [Header("---UI InputFields---")]
    public InputField createRoom;
    public InputField joinRoom;
    public InputField userName;
    public Button startButton;

    UIManager uiManager;

    //On-Click values
    public int sceneNumber;
    public Image roomImage;
    public Sprite map1Image;
    public Sprite map2Image;
    public Sprite map3Image;

    public FirebaseManager fb;
    string name;
    string id;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        roomImage.sprite = map1Image;
        sceneNumber = 1;
}
    private void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    private void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    public override void OnConnectedToMaster()
    {
        connectingText.text = "Joining Lobby...";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    public override void OnJoinedLobby()
    {
        statusText.text = "Joined to Lobby";
    }
    public override void OnJoinedRoom()
    {
        int sizeOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        AssignTeam(sizeOfPlayers);
        UIManager.instance.RoomScreen();

       // PhotonNetwork.CurrentRoom.Players;
        foreach(Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if(p.NickName!=PhotonNetwork.LocalPlayer.NickName)
                GetComponent<LobbyUIManager>().AddPlayer(p.NickName);
        }
        GetComponent<LobbyUIManager>().AddPlayer(PhotonNetwork.LocalPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            startBtnText.text = "Waiting for players...";
        }
        else
        {
            startBtnText.text = "Ready!";
        }

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        connectingText.text = "Disconnected... "+cause.ToString();
        roomUI.SetActive(false);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int roomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomName.ToString(), roomOptions, TypedLobby.Default, null);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        GetComponent<LobbyUIManager>().AddPlayer(newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        GetComponent<LobbyUIManager>().RemovePlayer(otherPlayer.NickName);
    }
    #region ButtonClicks
    public void Onclick_CreateRoomMenuBtn()
    {
        UIManager.instance.CreateRoomScreen();
    }
    public void Onclick_JoinRoomMenuBtn()
    {
        UIManager.instance.JoinRoomScreen();
    }
    public void Onclick_CreateBtn()
    {
        name = fb.user.DisplayName;
        id = fb.user.UserId;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.LocalPlayer.NickName = name;
        PhotonNetwork.CreateRoom(createRoom.text,roomOptions, TypedLobby.Default,null);
    }
    public void Onclick_JoinBtn()
    {
        name = fb.user.DisplayName;
        id = fb.user.UserId;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.LocalPlayer.NickName = name;
        PhotonNetwork.JoinOrCreateRoom(joinRoom.text, roomOptions, TypedLobby.Default, null);
    }
    public void Onclick_PlayNow()
    {
        name = fb.user.DisplayName;
        id = fb.user.UserId;
        PhotonNetwork.LocalPlayer.NickName = name;

        PhotonNetwork.JoinRandomRoom();
        statusText.text = "Creating a room, please wait...";

    }
    public void Onclick_CancelBtn()
    {
        GetComponent<LobbyUIManager>().RemovePlayer(PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LeaveRoom();
        UIManager.instance.LobbyScreen();
    }
    public void Onclick_Map1Btn()
    {
        roomImage.sprite = map1Image;
        sceneNumber = 1;
    }
    public void Onclick_Map2Btn()
    {
        roomImage.sprite = map2Image;
        sceneNumber = 2;
    }
    public void Onclick_Map3Btn()
    {
        roomImage.sprite = map3Image;
        sceneNumber = 3;
    }
    #endregion
    #region My_Functions
    void AssignTeam(int sizeOfPlayer)
    {
        Hashtable hash = new Hashtable();
        if (sizeOfPlayer % 2 == 0)
        {
            hash.Add("Team", 1);
        }
        else
        {
            hash.Add("Team", 2);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void OnClickStartButton()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            SendMsg();
            startButton.interactable = false;
            startBtnText.text = "Wait...";
        }
        else
        {
            if (count <= 4)
            {
                lobbyText.text = "Loading the game...";
                PhotonNetwork.LoadLevel(sceneNumber);
            }
        }
    }
    #endregion
    #region Raise_Events
    enum EventCodes 
    {
        ready=1
    }

    int count = 1;
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object content = photonEvent.CustomData;
        EventCodes code = (EventCodes)eventCode;

        if (code == EventCodes.ready)
        {
            object[] datas = content as object[];
            if (PhotonNetwork.IsMasterClient)
            {
                count++;
                if (count >= 2)
                    startBtnText.text = "START!";
                else
                    startBtnText.text = "Only " + count + "/ 4 players are Ready.";
            }
        }
    }

    public void SendMsg()
    {
        string message = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        object[] datas = new object[] { message };
        RaiseEventOptions options = new RaiseEventOptions
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.MasterClient
        };
        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent((byte)EventCodes.ready, datas, options, sendOptions);
    }
    #endregion
}
