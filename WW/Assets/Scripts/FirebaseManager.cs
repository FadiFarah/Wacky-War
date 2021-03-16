using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System;
using System.Linq;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //Lobby variables
    [Header("Lobby")]
    public TMP_InputField usernameLobbyField;
    public TMP_InputField xpLobbyField;
    public GameObject scoreElement;
    public Transform scoreboardContent;

    //Profile variables
    [Header("Profile")]
    public TMP_InputField usernameProfileField;
    public TMP_InputField xpProfileField;
    public TMP_InputField killsProfileField;
    public TMP_InputField deathsProfileField;

    //Info variables
    [Header("Info")]
    public TMP_InputField usernameInfoField;
    public TMP_InputField passwordInfoField;

    [Header("ForgotPass")]
    public TMP_InputField emailForgotPassField;
    public TMP_Text confirmForgotPassText;

    private void Awake()
    {
        
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
    private void Start()
    {
        DontDestroyOnLoad(GameObject.Find("FirebaseManager"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("UIManager"));
    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
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
    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text,usernameRegisterField.text));
    }

    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
    }

    //Function for the save button
    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameInfoField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameInfoField.text));
        StartCoroutine(UpdatePasswordAuth(passwordInfoField.text));
    }
    public void SendPassResetButton()
    {
        StartCoroutine(PasswordResetEmail(emailForgotPassField.text));
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
    private IEnumerator Login(string _email, string _password)
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

            yield return new WaitForSeconds(2);

            
            UIManager.instance.LobbyScreen();
           
            confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
        }
    }
    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
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
                        ClearLoginFields();
                        ClearRegisterFields();
                        StartCoroutine(UpdateUsernameDatabase(_username));
                        StartCoroutine(UpdateDeaths(0));
                        StartCoroutine(UpdateKills(0));
                        StartCoroutine(UpdateXp(1));
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
    private IEnumerator UpdateUsernameDatabase(string _username)
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

    private IEnumerator UpdateXp(int _xp)
    {
        //Set the currently logged in user xp
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("xp").SetValueAsync(_xp);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Xp is now updated
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

            string currentkills = snapshot.Child("kills").Value.ToString();
            int kills = Convert.ToInt32(currentkills)+1;
            StartCoroutine(UpdateKills(kills));

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

            string currentdeaths = snapshot.Child("deaths").Value.ToString();
            int deaths = Convert.ToInt32(currentdeaths)+1;
            StartCoroutine(UpdateDeaths(deaths));

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
            xpLobbyField.text = "0";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            usernameLobbyField.text = user.DisplayName;
            xpLobbyField.text = snapshot.Child("xp").Value.ToString();

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
            xpProfileField.text = "0";
            killsProfileField.text = "0";
            deathsProfileField.text = "0";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            usernameProfileField.text = user.DisplayName;
            xpProfileField.text = snapshot.Child("xp").Value.ToString();
            killsProfileField.text= snapshot.Child("kills").Value.ToString();
            deathsProfileField.text= snapshot.Child("deaths").Value.ToString();

        }
        UIManager.instance.ProfileScreen();
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
                int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());

                //Instantiate new scoreboard elements
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, kills, deaths, xp);
            }

            //Go to scoareboard screen
            UIManager.instance.ScoreboardScreen();
        }
    }
}
