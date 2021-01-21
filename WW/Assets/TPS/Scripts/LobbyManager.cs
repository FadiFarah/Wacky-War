using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("---UI Screens---")]
    public GameObject roomUI;
    public GameObject connectUI;

    [Header("---UI Text---")]
    public Text statusText;
    public Text connectingText;

    [Header("---UI InputFields---")]
    public InputField createRoom;
    public InputField joinRoom;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        connectingText.text = "Joining Lobby...";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    public override void OnJoinedLobby()
    {
        connectUI.SetActive(false);
        roomUI.SetActive(true);
        statusText.text = "Joined to Lobby";
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        connectUI.SetActive(true);
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
    #region ButtonClicks
    public void Onclick_CreateBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createRoom.text,roomOptions, TypedLobby.Default,null);
    }
    public void Onclick_JoinBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(joinRoom.text, roomOptions, TypedLobby.Default, null);
    }
    public void Onclick_PlayNow()
    {
        PhotonNetwork.JoinRandomRoom();
        statusText.text = "Creating a room, please wait...";

    }
    #endregion
}
