using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    public PlayerModel player;
    PlayerView playerView;

    [Header("Movement/Rotation")]
    public GameObject joystickBar;
    FixedJoystick joystick;
    public GameObject zipLine;
    public FixedZipLineButton zipLineBtn;
    private CharacterController characterController;

    [Header("Settings")]
    public TMP_Text graphicsSettingsText;
    public Slider musicVolumeSettingsSlider;
    public Slider inGameVolumeSettingsSlider;

    GameView gameView;
    GameManager gameManager;
    MusicSoundsManager musicSoundsManager;
    public int kills = 0;
    public string triggerName;
    private void Start()
    {
        player = new PlayerModel();

        if (photonView.IsMine)
        {
            joystickBar.SetActive(true);
            joystick = GameObject.Find("Joystick").GetComponent<FixedJoystick>();
            playerView = GetComponent<PlayerView>();
            characterController = GetComponent<CharacterController>();
            musicSoundsManager = GameObject.Find("MusicSounds").GetComponent<MusicSoundsManager>();
            zipLineBtn.SetPlayer(this);

        }
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameView = GameObject.Find("GameMVC").GetComponent<GameView>();

    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            Vector2 input = new Vector3(joystick.input.x, joystick.input.y);
            Vector2 inputDir = input.normalized;
            //Movement
            float currentSpeed = new Vector3(joystick.Horizontal, joystick.Vertical).magnitude;
            if (!player.climbingLadder && !player.zipLine)
            {
                player.SetPosition(player.posX + (input.x * Time.deltaTime * player.speed), player.posY, player.posZ + (input.y * Time.deltaTime * player.speed));
                playerView.Move(currentSpeed * 1);


                //Rotation
                if (inputDir != Vector2.zero)
                {
                    float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    player.SetRotation(player.rotX, player.rotY, player.rotZ);
                    playerView.Rotate(rotation);

                }
            }
            else if (player.climbingLadder)
            {
                if (triggerName == "StartTriggerLadder1")
                {
                    player.SetPosition(gameManager.ladder1Position.transform.position.x, player.posY + (inputDir.y * Time.deltaTime * player.speed), gameManager.ladder1Position.transform.position.z);
                    player.SetRotation(gameManager.ladder1Position.transform.eulerAngles.x, gameManager.ladder1Position.transform.eulerAngles.y, gameManager.ladder1Position.transform.eulerAngles.z);
                    playerView.ClimbLadder(inputDir.y);
                    GameObject [] LadderElevator = GameObject.FindGameObjectsWithTag("LadderElevator");
                    for(int i=0;i<LadderElevator.Length;i++)
                        LadderElevator[i].transform.position = new Vector3(LadderElevator[i].transform.position.x, LadderElevator[i].transform.position.y + (inputDir.y * Time.deltaTime * 2f * player.speed), LadderElevator[i].transform.position.z);
                }
                if (triggerName == "EndTriggerLadder1")
                {
                    playerView.StopClimbLadder();
                }
            }
            else if (player.zipLine)
            {

                Debug.Log("Zipping Controller");
                player.SetPosition(player.posX + (Time.deltaTime * player.speed*5), player.posY, player.posZ);
                player.SetRotation(player.rotX, player.rotY, player.rotZ);
                playerView.ZipLine();
                if (triggerName == "EndZipLine")
                {
                    playerView.StopZipLine();
                }
            }
        }
    }
    [PunRPC]
    public void GetDamage(float amount)
    {

        player.health -= amount;
        if (photonView.IsMine)
            playerView.Health();
        if (player.health <= 0 && photonView.IsMine)
        {
            Death();
        }
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
        if (player.health <= 0)
        {
            StartCoroutine(gameView.GetDeaths());
            photonView.RPC("SetDeaths", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void SetDeaths()
    {
        player.health = 1;
        player.Deaths++;
        Debug.Log(player.Deaths);
        Debug.Log(gameView.user.DisplayName + " is dead");

        if (photonView.IsMine)
        {
            photonView.RPC("Death", RpcTarget.AllBuffered);
            GameObject spawnPos = gameManager.GetSpawnPosition();
            player.SetPosition(spawnPos.transform.position.x, spawnPos.transform.position.y, spawnPos.transform.position.z);
            player.SetRotation(spawnPos.transform.rotation.x, spawnPos.transform.rotation.y, spawnPos.transform.rotation.z);
            playerView.StopClimbLadder();
            StartCoroutine(playerView.MoveToSpawnPosition());
        }

    }
    [PunRPC]
    public void SetKills()
    {
        kills++;
        if (photonView.IsMine)
        {
            player.Kills++;
            playerView.SetYouKills(player.Kills);
        }
    }

    [PunRPC]
    public void SetMostKills(int _kills, bool maxReached, bool timeEnded)
    {

        if (photonView.IsMine)
        {
            playerView.SetMostKills(_kills);
            if (maxReached == true && timeEnded == true)
            {
                if (kills >= _kills)
                {
                    player.Kills = -1;
                    SetWinner();
                }
                else if (player.Kills >= 0 && player.Kills < _kills)
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
        if (photonView.IsMine)
            playerView.ChatButton();
    }
    public void ChatMessageButton(string msg)
    {
        gameView.SendMsg(gameView.user.DisplayName + ": " + msg);
    }
    public void PauseButton()
    {
        if (photonView.IsMine)
        {
            musicSoundsManager.UIClick1();
            playerView.PauseButton();
            PlayerController playercontroller = GetComponent<PlayerController>();
            gameView.PauseMenuDataButton(playercontroller);
        }
    }
    public void PlayButton()
    {
        if (photonView.IsMine)
        {
            musicSoundsManager.UIClick1();
            playerView.PlayButton();
        }
    }
    public void ContinueButton()
    {
        musicSoundsManager.UIClick2();
        gameView.LeaveOrContinueButton();
    }
    public void SaveChangesButton()
    {
        musicSoundsManager.UIClick2();
        GameModel gameModel = new GameModel();
        GameObject.Find("GameMVC").GetComponent<GameController>().gameModel = gameModel;
        gameModel.OnSaveSettings(graphicsSettingsText.text, inGameVolumeSettingsSlider.value, musicVolumeSettingsSlider.value);
        gameView.SaveSettingsButton();
    }
    private void OnTriggerEnter(Collider trigger)
    {
        if (photonView.IsMine)
        {
            if (trigger.tag == "StartLadder")
                player.climbingLadder = true;
            if (trigger.tag == "StartZipLine")
            {
                zipLine.SetActive(true);
            }
            triggerName = trigger.name;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "StartZipLine")
        {
            zipLine.SetActive(false);
        }
    }

}
