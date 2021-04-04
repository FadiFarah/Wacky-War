using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController:MonoBehaviourPun
{
    public PlayerModel player;
    PlayerView playerView;
    public GameObject joystickBar;
    FixedJoystick joystick;
    private CharacterController characterController;

    [Header("Settings")]
    public TMP_Text graphicsSettingsText;
    public Slider musicVolumeSettingsSlider;
    public Slider inGameVolumeSettingsSlider;

    GameView gameView;

    private void Start()
    {
            player = new PlayerModel();

        if (photonView.IsMine)
        {
            joystickBar.SetActive(true);
            joystick = GameObject.Find("Joystick").GetComponent<FixedJoystick>();
            playerView = GetComponent<PlayerView>();
            characterController = GetComponent<CharacterController>();
        }
        gameView = GameObject.Find("GameMVC").GetComponent<GameView>();
        
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            Vector2 input = new Vector3(joystick.input.x,  joystick.input.y);
            Vector2 inputDir = input.normalized;
            //Movement
            float currentSpeed = new Vector3(joystick.Horizontal, joystick.Vertical).magnitude;
            if (!characterController.isGrounded)
            {
                player.posY += Time.deltaTime * player.gravity;
            }
            else
                player.posY = 0;

              player.SetPosition(player.posX + (input.x * Time.deltaTime * player.speed), player.posY, player.posZ + (input.y * Time.deltaTime * player.speed));
              playerView.Move(currentSpeed*1);


            //Rotation
            if (inputDir!=Vector2.zero)
            {
                float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                player.SetRotation(player.rotX, player.rotY, player.rotZ);
                playerView.Rotate(rotation);
            }
        }
    }
    [PunRPC]
    public void GetDamage(float amount)
    {

        player.health -= amount;
        if (player.health <= 0 && photonView.IsMine)
        {
            Death();
        }
        if(photonView.IsMine)
            playerView.Health();
    }
    public void Kill()
    {
        StartCoroutine(gameView.GetKills());
        Debug.Log(gameView.user.DisplayName + " got a kill");
        photonView.RPC("SetKills", RpcTarget.AllBuffered);
        Debug.Log(player.Kills);

    }
    void Death()
    {
        //anim.SetTrigger("death");
        //photonView.RPC("Hideplayer",RpcTarget.All);
        StartCoroutine(gameView.GetDeaths());
        Debug.Log(gameView.user.DisplayName + " is dead");
        player.Deaths++;
        Debug.Log(player.Deaths);
    }
    [PunRPC]
    public void SetKills()
    {
        player.Kills++;
        if (photonView.IsMine)
            playerView.SetYouKills(player.Kills);
    }

    [PunRPC]
    public void SetMostKills(int _kills, bool maxReached, bool timeEnded)
    {

        if (photonView.IsMine)
        {
            playerView.SetMostKills(_kills);
            if (maxReached == true && timeEnded == true)
            {
                if (player.Kills >= _kills)
                {
                    player.Kills = -1;
                    SetWinner();
                }
                else if(player.Kills>=0 && player.Kills<_kills)
                {
                    player.Kills = -1;
                    SetLoser();
                }
            }
        }
    }

    public void SetYouKills()
    {
        if (photonView.IsMine)
        {
            playerView.SetYouKills(player.Kills);
        }
    }

    public void SetWinner()
    {
        if (photonView.IsMine)
        {
            playerView.SetWinner();
            StartCoroutine(gameView.GetWins());
        }

    }
    public void SetLoser()
    {
        if (photonView.IsMine)
        {
            playerView.SetLoser();
            StartCoroutine(gameView.GetLoses());

        }
    }
    public void ChatButton()
    {
        if(photonView.IsMine)
            playerView.ChatButton();
    }
    public void ChatMessageButton(string msg)
    {
        gameView.SendMsg(gameView.user.DisplayName+": "+ msg);
    }
    public void PauseButton()
    {
        if (photonView.IsMine)
        {
            playerView.PauseButton();
            PlayerController playercontroller = GetComponent<PlayerController>();
            gameView.PauseMenuDataButton(playercontroller);
        }
    }
    public void PlayButton()
    {
        if (photonView.IsMine)
            playerView.PlayButton();
    }
    public void ContinueButton()
    {
        gameView.LeaveOrContinueButton();
    }
    public void SaveChangesButton()
    {
        GameModel gameModel = new GameModel();
        GameObject.Find("GameMVC").GetComponent<GameController>().gameModel=gameModel;
        gameModel.OnSaveSettings(graphicsSettingsText.text, inGameVolumeSettingsSlider.value, musicVolumeSettingsSlider.value);
        gameView.SaveSettingsButton();
    }

}
