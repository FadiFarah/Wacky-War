using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{


    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;

    //Info variables
    [Header("Info")]
    public TMP_InputField usernameInfoField;
    public TMP_InputField passwordInfoField;

    [Header("Settings")]
    public TMP_Text graphicsSettingsText;
    public Slider musicVolumeSettingsSlider;
    public Slider inGameVolumeSettingsSlider;

    [Header("ForgotPass")]
    public TMP_InputField emailForgotPassField;

    [Header("Create Room")]
    public InputField createRoomNameField;
    public int sceneNumber=1;
    public int maxPlayers=4;

    [Header("Join Room")]
    public TMP_InputField joinRoomNameField;

    [Header("In-Room")]
    //public InputField RoomuserNameField;
    public Button RoomStartButton;


    public GameModel gameModel;
    public GameView gameView;
    // Start is called before the first frame update

    private void Start()
    {
        gameModel = new GameModel();
    }
    public void LoginButton()
    {
        gameModel = new GameModel();
        gameModel.OnLogin(emailLoginField.text, passwordLoginField.text);
        gameView.LoginButton();
    }
    public void RegisterButton()
    {
        gameModel = new GameModel();
        gameModel.OnRegister(emailRegisterField.text, passwordRegisterField.text,passwordRegisterVerifyField.text, usernameRegisterField.text);
        gameView.RegisterButton();
    }
    public void SaveDataButton()
    {
        gameModel = new GameModel();
        gameModel.OnSaveData(usernameInfoField.text, passwordInfoField.text);
        gameView.SaveDataButton();
    }
    public void SaveAvatarButton(string avatarname)
    {
        gameModel = new GameModel();
        gameModel.OnSaveAvatar(avatarname);
        gameView.SaveAvatarButton();
    }
    public void SaveSettingsButton()
    {
        gameModel = new GameModel();
        gameModel.OnSaveSettings(graphicsSettingsText.text, inGameVolumeSettingsSlider.value, musicVolumeSettingsSlider.value);
        gameView.SaveSettingsButton();
    }
    public void SendPassResetButton()
    {
        gameModel = new GameModel();
        gameModel.OnPassReset(emailForgotPassField.text);
        gameView.SendPassResetButton();
    }

    public void CreateRoomButton()
    {
        gameModel = new GameModel();
        gameModel.OnCreateRoom(sceneNumber, maxPlayers, createRoomNameField.text);
        gameView.CreateRoomButton();
    }
    public void JoinRoomButton()
    {
        gameModel = new GameModel();
        gameModel.OnJoinRoom(1,4,joinRoomNameField.text);
        gameView.JoinRoomButton();
    }
    public void StartButton()
    {
        gameView.StartButton();
    }
    public void PlayNowButton()
    {
        gameModel = new GameModel();
        gameModel.OnPlayNow(1,4, Random.Range(0, 1000).ToString());
        gameView.PlayNowButton();
    }
    public void CancelButton()
    {
        gameView.CancelButton();
    }
    public void LeaveRoomButton()
    {
        gameView.LeaveRoomButton();
    }
    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }
    public void Map1Button()
    {
        sceneNumber = 1;
        gameView.Map1Button();
    }
    public void Map2Button()
    {
        sceneNumber = 2;
        gameView.Map2Button();
    }
    public void Map3Button()
    {
        sceneNumber = 3;
        gameView.Map3Button();
    }
    public void FourPlayersButton()
    {
        maxPlayers = 4;
    }
    public void SixPlayersButton()
    {
        maxPlayers = 6;
    }
    public void EightPlayersButton()
    {
        maxPlayers = 8;
    }
}
