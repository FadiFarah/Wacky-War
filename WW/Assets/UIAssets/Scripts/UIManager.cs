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
    public Animator UIAnimator;


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
        StartCoroutine(ActivateTransition(loginUI));
    }
    public void RegisterScreen() 
    {
        ClearScreen();
        StartCoroutine(ActivateTransition(registerUI));
    }
    public void LobbyScreen() 
    {
        ClearScreen();
        StartCoroutine(ActivateTransition(lobbyUI));
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
        StartCoroutine(ActivateTransition(scoreboardUI));
    }
    public void JoinRoomScreen() 
    {
        ClearScreen();
        StartCoroutine(ActivateTransition(joinroomUI));
    }
    public void CreateRoomScreen() 
    {
        ClearScreen();
        StartCoroutine(ActivateTransition(createroomUI));
    }
    public void SettingsScreen()
    {
        ClearScreen();
        StartCoroutine(ActivateTransition(settingsUI));
    }
    public void RoomScreen() 
    {
        ClearScreen();
        StartCoroutine(ActivateTransition(roomUI));
    }
    public void ForgotPassScreen()
    {
        ClearScreen();
        StartCoroutine(ActivateTransition(forgotpassUI));
    }
    public IEnumerator ActivateTransition(GameObject bar)
    {
        UIAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(0.4f);
        UIAnimator.SetTrigger("End");
        bar.SetActive(true);
    }
}
