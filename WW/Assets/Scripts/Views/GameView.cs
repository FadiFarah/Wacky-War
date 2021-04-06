using ExitGames.Client.Photon;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameView : MonoBehaviourPunCallbacks
{

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DBreference;

    public GameModel gameModel;
    public GameController gameController;

    //warnings/confirms
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;
    public TMP_Text warningRegisterText;
    public TMP_Text confirmForgotPassText;

    //Lobby variables
    [Header("Lobby")]
    public TMP_InputField usernameLobbyField;
    public TMP_InputField levelLobbyField;
    public Image avatarLobbyImage;
    public Image expLobbyFillImage;
    public GameObject scoreElement;
    public Transform scoreboardContent;

    //Profile variables
    [Header("Profile")]
    public TMP_InputField usernameProfileField;
    public TMP_InputField levelProfileField;
    public TMP_InputField killsProfileField;
    public TMP_InputField deathsProfileField;
    public TMP_InputField rateProfileField;
    public TMP_InputField winsProfileField;
    public TMP_InputField losesProfileField;
    public Image avatarProfileImage;

    [Header("Settings")]
    //graphics
    public RenderPipelineAsset high;
    public RenderPipelineAsset medium;
    public RenderPipelineAsset low;
    public TMP_Text graphicsSettingsText;
    //sounds
    public AudioMixer masterVolumeMixer;
    public Slider musicVolumeSettingsSlider;
    public Slider inGameVolumeSettingsSlider;

    [Header("Room")]
    public Text RoomStartBtnText;
    public Image roomImage;
    public Sprite map1Image;
    public Sprite map2Image;
    public Sprite map3Image;
    public GameObject playersContainer;
    public GameObject playerAvatarPrefab;

    bool startedgame = false;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 25; //20 default
        PhotonNetwork.SerializationRate = 10;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    void Start()
    {
        DontDestroyOnLoad(GameObject.Find("GameMVC"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("UIManager"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
    }
    #region Database Connections

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void LoginButton()
    {
        gameModel = gameController.gameModel;
        StartCoroutine(Login(gameModel.Email, gameModel.Password));
    }
    public void RegisterButton()
    {
        gameModel = gameController.gameModel;
        StartCoroutine(Register(gameModel.Email, gameModel.Password, gameModel.ConfirmPassword, gameModel.Username));
    }
    public void SaveDataButton()
    {
        gameModel = gameController.gameModel;
        StartCoroutine(UpdateUsernameAuth(gameModel.Username));
        StartCoroutine(UpdateUsernameDatabase(gameModel.Username));
        StartCoroutine(UpdatePasswordAuth(gameModel.Password));
    }
    public void SaveAvatarButton()
    {
        gameModel = gameController.gameModel;
        StartCoroutine(UpdateAvatarDatabase(gameModel.AvatarName));
    }
    public void SaveSettingsButton()
    {
        gameModel = gameController.gameModel;
        StartCoroutine(UpdateGraphics(gameModel.Graphics));
        StartCoroutine(UpdateMusicVolume(gameModel.MusicVolume));
        StartCoroutine(UpdateInGameVolume(gameModel.InGameVolume));
    }
    public void SendPassResetButton()
    {
        gameModel = gameController.gameModel;
        StartCoroutine(PasswordResetEmail(gameModel.Email));
    }
    //Function for the Profile button
    public void ProfileDataButton()
    {
        StartCoroutine(LoadProfileData());
    }
    //Function for the lobby button
    public void LobbyDataButton()
    {
        StartCoroutine(LoadLobbyData());
    }

    //Function for the scoreboard button
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }
    //Function for the Settings button
    public void SettingsDataButton()
    {
        StartCoroutine(LoadSettingsData());
    }
    public void PauseMenuDataButton(PlayerController playercontroller)
    {
        StartCoroutine(LoadPauseMenuData(playercontroller));
    }
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        gameController.ClearRegisterFields();
        gameController.ClearLoginFields();
    }
    public IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth function and passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            user = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(LoadLobbyData());
            StartCoroutine(GetSettings());

            yield return new WaitForSeconds(2);


            confirmLoginText.text = "";
            gameController.ClearLoginFields();
            gameController.ClearRegisterFields();
        }
    }

    private IEnumerator Register(string _email, string _password, string _confirmPassword, string _username)
    {
        if (_username.Length < 3 || _username.Length > 20)
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Username cannot contain less than 3 or bigger than 20 characters";
        }
        else if (_password != _confirmPassword)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                user = RegisterTask.Result;

                if (user != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        gameController.ClearLoginFields();
                        gameController.ClearRegisterFields();
                        StartCoroutine(UpdateAvatarDatabase("firstavatar"));
                        StartCoroutine(UpdateUsernameDatabase(_username));
                        StartCoroutine(UpdateDeaths(0));
                        StartCoroutine(UpdateKills(0));
                        StartCoroutine(UpdateExp(0));
                        StartCoroutine(UpdateLevel(1));
                        StartCoroutine(UpdateWins(0));
                        StartCoroutine(UpdateLoses(0));
                        StartCoroutine(UpdateRate(0, 0));
                        StartCoroutine(UpdateInGameVolume(0));
                        StartCoroutine(UpdateMusicVolume(0));
                        StartCoroutine(UpdateGraphics("HIGH"));
                    }
                }
            }
        }
    }

    private IEnumerator PasswordResetEmail(string _email)
    {
        var MailTask = auth.SendPasswordResetEmailAsync(_email);
        yield return new WaitUntil(predicate: () => MailTask.IsCompleted);

        if (MailTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {MailTask.Exception}");
        }
        else
        {
            //Password reset email is sent
            confirmForgotPassText.text = "An email has been sent!";
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        if (_username.Length < 3 || _username.Length > 20)
        {
            Debug.LogWarning("Username does not meet the correct requirements!");
        }
        else
        {
            //Create a user profile and set the username
            UserProfile profile = new UserProfile { DisplayName = _username };
            //Call the Firebase auth update user profile function passing the profile with the username
            var ProfileTask = user.UpdateUserProfileAsync(profile);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
            }
            else
            {
                //Auth username is now updated
            }
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        if (_username.Length < 3 || _username.Length > 20)
        {
            Debug.LogWarning("Username does not meet the correct requirements!");
        }
        else
        {
            var DBTask = DBreference.Child("users").Child(user.UserId).Child("username").SetValueAsync(_username);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Database username is updated
            }
        }
    }

    private IEnumerator UpdatePasswordAuth(string _password)
    {

        //Call the Firebase auth update user password function passing the password
        var ProfileTask = user.UpdatePasswordAsync(_password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            Debug.Log("Password changed");
            //Auth password is now updated
        }
    }
    private IEnumerator UpdateAvatarDatabase(string _avatarname)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("avatarname").SetValueAsync(_avatarname);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database avatar is updated
        }
    }

    public IEnumerator GetKills()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            int currentkills = int.Parse(snapshot.Child("kills").Value.ToString()) + 1;
            int currentdeaths = int.Parse(snapshot.Child("deaths").Value.ToString());
            int currentexp = int.Parse(snapshot.Child("exp").Value.ToString()) + 50;
            int currentlevel = int.Parse(snapshot.Child("level").Value.ToString());
            if (currentexp >= (Math.Pow(currentlevel, 2) * 100))
            {
                currentlevel++;
                StartCoroutine(UpdateLevel(currentlevel));
            }
            StartCoroutine(UpdateKills(currentkills));
            StartCoroutine(UpdateExp(currentexp));
            StartCoroutine(UpdateRate(currentkills, currentdeaths));

        }
    }
    public IEnumerator GetDeaths()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            int currentdeaths = int.Parse(snapshot.Child("deaths").Value.ToString()) + 1;
            int currentkills = int.Parse(snapshot.Child("kills").Value.ToString());
            StartCoroutine(UpdateDeaths(currentdeaths));
            StartCoroutine(UpdateRate(currentkills, currentdeaths));

        }
    }
    public IEnumerator GetWins()
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            int currentwins = int.Parse(snapshot.Child("wins").Value.ToString()) + 1;
            StartCoroutine(UpdateWins(currentwins));
        }
    }

    public IEnumerator GetLoses()
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            int currentloses = int.Parse(snapshot.Child("loses").Value.ToString()) + 1;
            StartCoroutine(UpdateLoses(currentloses));
        }
    }
    public IEnumerator GetSettings()
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("settings").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            string currentgraphics = snapshot.Child("graphics").Value.ToString();
            float currentmusicVol = float.Parse(snapshot.Child("musicvolume").Value.ToString());
            float currentingameVol = float.Parse(snapshot.Child("ingamevolume").Value.ToString());
            StartCoroutine(UpdateGraphics(currentgraphics));
            StartCoroutine(UpdateMusicVolume(currentmusicVol));
            StartCoroutine(UpdateInGameVolume(currentingameVol));
            yield return new WaitForSeconds(2);
        }
    }
    private IEnumerator UpdateExp(int _exp)
    {
        //Set the currently logged in user xp
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("exp").SetValueAsync(_exp);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //exp is now updated
        }
    }
    private IEnumerator UpdateLevel(int _level)
    {
        //Set the currently logged in user xp
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("level").SetValueAsync(_level);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //level is now updated
        }
    }
    private IEnumerator UpdateKills(int _kills)
    {
        //Set the currently logged in user kills

        var DBTask = DBreference.Child("users").Child(user.UserId).Child("kills").SetValueAsync(_kills);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Kills are now updated
        }
    }

    private IEnumerator UpdateDeaths(int _deaths)
    {
        //Set the currently logged in user deaths
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("deaths").SetValueAsync(_deaths);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }
    private IEnumerator UpdateWins(int _wins)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("wins").SetValueAsync(_wins);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Wins are now updated
        }
    }
    private IEnumerator UpdateLoses(int _loses)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("loses").SetValueAsync(_loses);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Loses are now updated
        }
    }

    private IEnumerator UpdateRate(int _kills, int _deaths)
    {
        if (_deaths == 0)
            _deaths = 1;
        float _rate = (float)_kills / (float)_deaths;
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("rate").SetValueAsync(_rate);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Rate is now updated
        }
    }

    private IEnumerator UpdateGraphics(string _graphics)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("settings").Child("graphics").SetValueAsync(_graphics);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Graphics are now updated
            if (_graphics.ToUpper() == "HIGH")
            {
                GraphicsSettings.renderPipelineAsset = high;
                Debug.Log("Default render pipeline asset is: " + GraphicsSettings.renderPipelineAsset.name);
            }
            else if (_graphics.ToUpper() == "MEDIUM")
            {
                GraphicsSettings.renderPipelineAsset = medium;
                Debug.Log("Default render pipeline asset is: " + GraphicsSettings.renderPipelineAsset.name);
            }
            else if (_graphics.ToUpper() == "LOW")
            {
                GraphicsSettings.renderPipelineAsset = low;
                Debug.Log("Default render pipeline asset is: " + GraphicsSettings.renderPipelineAsset.name);
            }
        }
    }
    private IEnumerator UpdateMusicVolume(float _musicvol)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("settings").Child("musicvolume").SetValueAsync(_musicvol);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Music Vol are now updated

            masterVolumeMixer.SetFloat("musicVolume", _musicvol);
        }
    }
    private IEnumerator UpdateInGameVolume(float _ingamevol)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("settings").Child("ingamevolume").SetValueAsync(_ingamevol);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //InGame Vol are now updated
            masterVolumeMixer.SetFloat("inGameVolume", _ingamevol);
        }
    }

    private IEnumerator LoadLobbyData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            usernameLobbyField.text = user.DisplayName;
            levelLobbyField.text = "1";
            expLobbyFillImage.fillAmount = 0;
            avatarLobbyImage.sprite = Resources.Load<Sprite>("Sprites/firstavatar");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            string currentavatar = snapshot.Child("avatarname").Value.ToString();
            avatarLobbyImage.sprite = Resources.Load<Sprite>("Sprites/" + currentavatar);
            usernameLobbyField.text = user.DisplayName;
            double currentlevel = double.Parse(snapshot.Child("level").Value.ToString());
            levelLobbyField.text = currentlevel.ToString();
            double currentexp = double.Parse(snapshot.Child("exp").Value.ToString());
            double exptoreach = Math.Pow(currentlevel, 2) * 100;
            double startexp = Math.Pow(currentlevel - 1, 2) * 100;
            double difference = exptoreach - startexp;
            expLobbyFillImage.fillAmount = ((float)currentexp - (float)startexp) / (float)difference;
        }
        UIManager.instance.LobbyScreen();
    }

    private IEnumerator LoadProfileData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            usernameProfileField.text = user.DisplayName;
            levelProfileField.text = "1";
            killsProfileField.text = "0";
            deathsProfileField.text = "0";
            rateProfileField.text = "0.00";
            winsProfileField.text = "0";
            losesProfileField.text = "0";
            avatarProfileImage.sprite = Resources.Load<Sprite>("Sprites/firstavatar");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            string currentavatar = snapshot.Child("avatarname").Value.ToString();
            avatarProfileImage.sprite = Resources.Load<Sprite>("Sprites/" + currentavatar);
            usernameProfileField.text = user.DisplayName;
            levelProfileField.text = snapshot.Child("level").Value.ToString();
            killsProfileField.text = snapshot.Child("kills").Value.ToString();
            deathsProfileField.text = snapshot.Child("deaths").Value.ToString();
            rateProfileField.text = snapshot.Child("rate").Value.ToString();
            winsProfileField.text = snapshot.Child("wins").Value.ToString();
            losesProfileField.text = snapshot.Child("loses").Value.ToString();

        }
        UIManager.instance.ProfileScreen();
    }

    private IEnumerator LoadSettingsData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("settings").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            graphicsSettingsText.text = "HIGH";
            musicVolumeSettingsSlider.value = 0;
            inGameVolumeSettingsSlider.value = 0;
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            graphicsSettingsText.text = snapshot.Child("graphics").Value.ToString();
            musicVolumeSettingsSlider.value = float.Parse(snapshot.Child("musicvolume").Value.ToString());
            inGameVolumeSettingsSlider.value = float.Parse(snapshot.Child("ingamevolume").Value.ToString());

        }
        UIManager.instance.SettingsScreen();
    }

    private IEnumerator LoadPauseMenuData(PlayerController playercontroller)
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("settings").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            playercontroller.graphicsSettingsText.text = "HIGH";
            playercontroller.musicVolumeSettingsSlider.value = 0;
            playercontroller.inGameVolumeSettingsSlider.value = 0;
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            playercontroller.graphicsSettingsText.text = snapshot.Child("graphics").Value.ToString();
            playercontroller.musicVolumeSettingsSlider.value = float.Parse(snapshot.Child("musicvolume").Value.ToString());
            playercontroller.inGameVolumeSettingsSlider.value = float.Parse(snapshot.Child("ingamevolume").Value.ToString());

        }
    }

    private IEnumerator LoadScoreboardData()
    {
        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild("kills").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                int deaths = int.Parse(childSnapshot.Child("deaths").Value.ToString());
                int level = int.Parse(childSnapshot.Child("level").Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, kills, deaths, level);
            }

            //Go to scoareboard screen
            UIManager.instance.ScoreboardScreen();
        }
    }
    #endregion

    #region Network Connection
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
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UIManager.instance.LoginScreen();
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        //PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnJoinedRoom()
    {
        //int sizeOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        //AssignTeam(sizeOfPlayers);
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        // PhotonNetwork.CurrentRoom.Players;
        UIManager.instance.RoomScreen();
        foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (p.NickName != PhotonNetwork.LocalPlayer.NickName)
                AddPlayer(p.NickName, (string)p.CustomProperties["avatarname"]);
        }
        AddPlayer(PhotonNetwork.LocalPlayer.NickName, (string)PhotonNetwork.LocalPlayer.CustomProperties["avatarname"]);

        if (PhotonNetwork.IsMasterClient)
        {
            RoomStartBtnText.text = "Waiting for players...";
        }
        else
        {
            RoomStartBtnText.text = "Ready!";
        }


    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        gameModel = gameController.gameModel;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)gameModel.MaxPlayers;
        //roomOptions.CustomRoomProperties["maxkills"] = 20;
        PhotonNetwork.CreateRoom(gameModel.RoomName, roomOptions, TypedLobby.Default, null);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        AddPlayer(newPlayer.NickName, (string)newPlayer.CustomProperties["avatarname"]);
    }
    public override void OnLeftRoom()
    {
        if (startedgame)
            SceneManager.LoadSceneAsync(2);
        StartCoroutine(LoadLobbyData());
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        RemovePlayer(otherPlayer.NickName);
        Debug.Log(otherPlayer.NickName + " left the room");
        if (otherPlayer.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[1]);
        }
    }
    public void Onclick_CreateRoomMenuBtn()
    {
        UIManager.instance.CreateRoomScreen();
    }
    public void Onclick_JoinRoomMenuBtn()
    {
        UIManager.instance.JoinRoomScreen();
    }
    public void CreateRoomButton()
    {
        gameModel = gameController.gameModel;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)gameModel.MaxPlayers;
        //roomOptions.CustomRoomProperties.Add("maxkills",20);
        PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
        string avatarName = (string)PhotonNetwork.LocalPlayer.CustomProperties["avatarname"];
        avatarName = avatarLobbyImage.sprite.name;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("avatarname", avatarName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        PhotonNetwork.CreateRoom(gameModel.RoomName, roomOptions, TypedLobby.Default, null);
        //UIManager.instance.RoomScreen();

    }
    public void JoinRoomButton()
    {
        gameModel = gameController.gameModel;

        PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
        string avatarName = (string)PhotonNetwork.LocalPlayer.CustomProperties["avatarname"];
        avatarName = avatarLobbyImage.sprite.name;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("avatarname", avatarName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)gameModel.MaxPlayers;
        //roomOptions.CustomRoomProperties.Add("maxkills",20);
        PhotonNetwork.JoinOrCreateRoom(gameModel.RoomName, roomOptions, TypedLobby.Default, null);


    }
    public void PlayNowButton()
    {
        PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
        string avatarName = (string)PhotonNetwork.LocalPlayer.CustomProperties["avatarname"];
        avatarName = avatarLobbyImage.sprite.name;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("avatarname", avatarName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (PhotonNetwork.JoinRandomRoom())
        {
        }

    }
    public void StartButton()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            gameModel = gameController.gameModel;
            SendMsg(EventCodes.ready);
            gameController.RoomStartButton.interactable = false;
            RoomStartBtnText.text = "Wait...";
        }
        else
        {
            if (count <= 4)
            {
                gameModel = gameController.gameModel;
                SendMsg(EventCodes.start);
                PhotonNetwork.LoadLevel(gameModel.SceneNumber);

            }
        }

    }
    public void CancelButton()
    {
        UIManager.instance.LobbyScreen();
    }
    public void LeaveRoomButton()
    {
        startedgame = false;
        PhotonNetwork.LeaveRoom();
        foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            RemovePlayer(p.NickName);
        }
    }
    public void LeaveOrContinueButton()
    {

        startedgame = true;
        PhotonNetwork.LeaveRoom();
        gameController.RoomStartButton.interactable = true;
        count = 1;
        foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            RemovePlayer(p.NickName);
        }
    }
    public void Map1Button()
    {
        roomImage.sprite = map1Image;
    }
    public void Map2Button()
    {
        roomImage.sprite = map2Image;
    }
    public void Map3Button()
    {
        roomImage.sprite = map3Image;
    }
    void AssignTeam(int sizeOfPlayer)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
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

    public void AddPlayer(string playerName, string avatarname)
    {
        GameObject PaP = Instantiate(playerAvatarPrefab, Vector3.zero, Quaternion.identity);
        PaP.transform.GetChild(0).GetComponent<Text>().text = playerName;
        PaP.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + avatarname);
        PaP.transform.parent = playersContainer.transform;
        PaP.transform.localScale = Vector3.one;
        PaP.name = playerName;
    }
    public void RemovePlayer(string playerName)
    {
        int PaPCount = playersContainer.transform.childCount;
        for (int i = 0; i < PaPCount; i++)
        {
            if (playersContainer.transform.GetChild(i).name == playerName)
            {
                Destroy(playersContainer.transform.GetChild(i).gameObject);
                return;
            }
        }
    }
    #endregion
    #region Raise_Events
    public enum EventCodes
    {
        chatmessage = 0,
        ready = 1,
        start = 2,
        time = 3
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
                    RoomStartBtnText.text = "START!";
                else
                    RoomStartBtnText.text = "Only " + count + "/ 4 players are Ready.";
            }
        }
        if (code == EventCodes.chatmessage)
        {
            object[] datas = content as object[];
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                p.GetComponent<PlayerView>().chatMessage.text = (string)datas[0];
            }
        }
        if (code == EventCodes.start)
        {
            UIManager.instance.ClearScreen();
        }
        if (code == EventCodes.time)
        {
            object[] datas = content as object[];
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                gameManager.timeRemainTxt.text = (string)datas[0];
            }
        }
    }

    public void SendMsg(EventCodes eventcode)
    {
        RaiseEventOptions options;
        string message = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        object[] datas = new object[] { message };
        if (eventcode == EventCodes.ready)
        {
            options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.MasterClient
            };
        }
        else
        {
            options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.All
            };
        }

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;
        PhotonNetwork.RaiseEvent((byte)eventcode, datas, options, sendOptions);
    }
    public void SendMsg(string message)
    {
        object[] datas = new object[] { message };
        RaiseEventOptions options = new RaiseEventOptions
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;
        PhotonNetwork.RaiseEvent((byte)EventCodes.chatmessage, datas, options, sendOptions);
    }
    public void SendTime(string time)
    {
        object[] datas = new object[] { time };
        RaiseEventOptions options = new RaiseEventOptions
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;
        PhotonNetwork.RaiseEvent((byte)EventCodes.time, datas, options, sendOptions);
    }
    #endregion
}


