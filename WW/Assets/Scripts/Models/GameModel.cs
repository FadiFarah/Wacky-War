using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string AvatarName { get; set; }
    public string ConfirmPassword { get; set; }
    public string RoomName { get; set; }
    public int MaxPlayers { get; set; }
    public int SceneNumber { get; set; }
    public string Graphics { get; set; }
    public float InGameVolume { get; set; }
    public float MusicVolume { get; set; }


    public void OnLogin(string email,string password)
    {
        Email = email;
        Password = password;
    }
    public void OnRegister(string email, string password,string confirmPassword,string username)
    {
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
        Username = username;
    }
    public void OnSaveData(string username,string password)
    {
        Username = username;
        Password = password;
    }
    public void OnSaveAvatar(string avatarname)
    {
        AvatarName = avatarname;
    }
    public void OnSaveSettings(string graphics, float ingamevolume,float musicvolume)
    {
        Graphics = graphics;
        InGameVolume = ingamevolume;
        MusicVolume = musicvolume;
    }
    public void OnPassReset(string email)
    {
        Email = email;
    }
    public void OnCreateRoom(int sceneNumber,int maxPlayers, string roomName)
    {
        SceneNumber = sceneNumber;
        MaxPlayers = maxPlayers;
        RoomName = roomName;
    }
    public void OnJoinRoom(int sceneNumber, int maxPlayers, string roomName)
    {
        SceneNumber = sceneNumber;
        MaxPlayers = maxPlayers;
        RoomName = roomName;
    }
    public void OnPlayNow(int sceneNumber,int maxPlayers,string roomName)
    {
        SceneNumber = sceneNumber;
        MaxPlayers = maxPlayers;
        RoomName = roomName;
    }
}
