using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject lobbyUI;
    public GameObject profileUI;
    public GameObject infoUI;
    public GameObject settingsUI;
    public GameObject scoreboardUI;
    public GameObject joinroomUI;
    public GameObject createroomUI;
    public GameObject roomUI;
    public GameObject forgotpassUI;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI
    public void ClearScreen() //Turn off all screens
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        lobbyUI.SetActive(false);
        profileUI.SetActive(false);
        infoUI.SetActive(false);
        scoreboardUI.SetActive(false);
        joinroomUI.SetActive(false);
        createroomUI.SetActive(false);
        roomUI.SetActive(false);
        forgotpassUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    public void LoginScreen()
    {
        ClearScreen();
        loginUI.SetActive(true);
    }
    public void RegisterScreen() 
    {
        ClearScreen();
        registerUI.SetActive(true);
    }
    public void LobbyScreen() 
    {
        ClearScreen();
        lobbyUI.SetActive(true);
    }
    public void ProfileScreen() 
    {
        ClearScreen();
        profileUI.SetActive(true);
    }

    public void InfoScreen() 
    {
        ClearScreen();
        infoUI.SetActive(true);
    }
    public void ScoreboardScreen() 
    {
        ClearScreen();
        scoreboardUI.SetActive(true);
    }
    public void JoinRoomScreen() 
    {
        ClearScreen();
        joinroomUI.SetActive(true);
    }
    public void CreateRoomScreen() 
    {
        ClearScreen();
        createroomUI.SetActive(true);
    }
    public void SettingsScreen()
    {
        ClearScreen();
        settingsUI.SetActive(true);
    }
    public void RoomScreen() 
    {
        ClearScreen();
        roomUI.SetActive(true);
    }
    public void ForgotPassScreen()
    {
        ClearScreen();
        forgotpassUI.SetActive(true);
    }
}
