using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject lobbyUI;
    public GameObject profileUI;
    public GameObject infoUI;
    public GameObject scoreboardUI;

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
    }

    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
    }
    public void RegisterScreen() // Regester button
    {
        ClearScreen();
        registerUI.SetActive(true);
    }
    public void LobbyScreen() //Logged in
    {
        ClearScreen();
        lobbyUI.SetActive(true);
    }
    public void ProfileScreen() //Logged in
    {
        ClearScreen();
        profileUI.SetActive(true);
    }

    public void InfoScreen() //Logged in
    {
        ClearScreen();
        infoUI.SetActive(true);
    }
    public void ScoreboardScreen() //Scoreboard button
    {
        ClearScreen();
        scoreboardUI.SetActive(true);
    }
}
