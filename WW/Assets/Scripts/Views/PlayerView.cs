using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerView:MonoBehaviourPun
{
    PlayerModel playerModel;
    public Animator anim;
    public GameObject mostKillsBar;
    public TMP_Text mostKillsTxt;

    public GameObject youKillsBar;
    public TMP_Text youKillsTxt;

    public GameObject healthBar;
    public Image HealthfillImage;

    public GameObject crosshairBar;

    public GameObject jumpBar;

    public GameObject crouchBar;

    public GameObject slideBar;

    public GameObject matchResultFrameBar;
    public GameObject victoryImage;
    public GameObject defeatImage;

    public GameObject pauseBar;
    public GameObject pauseMenuBar;

    public GameObject chatSystemBar;
    public GameObject chatMessages;
    public Text chatMessage;

    //Deathcam
    public CameraController cameracontroller;
    public CharacterController characterController;
    public GameObject playerMeshes;
    public GameObject playerHips;

    public GameObject inGameSounds;
    public inGameSoundsManager inGameSoundsManager;

    float currentVelocity;

    void Start()
    {
        if (photonView.IsMine)
        {
            playerModel = GetComponent<PlayerController>().player;

            healthBar.SetActive(true);
            mostKillsBar.SetActive(true);
            jumpBar.SetActive(true);
            crouchBar.SetActive(true);
            slideBar.SetActive(true);
            crosshairBar.SetActive(true);
            youKillsBar.SetActive(true);
            mostKillsBar.SetActive(true);
            chatSystemBar.SetActive(true);
            pauseBar.SetActive(true);
            inGameSounds.SetActive(true);
            inGameSounds.transform.parent = null;
            GameObject.Find("Jump_Btn").GetComponent<FixedJumpButton>().SetPlayer(this);
            GameObject.Find("Crouch_Btn").GetComponent<FixedCrouchButton>().SetPlayer(this);
            GameObject.Find("Slide_Btn").GetComponent<FixedSlideButton>().SetPlayer(this);

        }
    }
    public void Move(float speed)
    {
        if (photonView.IsMine)
        {
           playerModel = GetComponent<PlayerController>().player; 
           transform.position = new Vector3(playerModel.posX, playerModel.posY, playerModel.posZ);
           anim.SetFloat("Speed", speed);
            anim.SetBool("climbingLadder", false);
        }
    }
    public void Rotate(float rotation)
    {
        if (photonView.IsMine)
        {
            playerModel = GetComponent<PlayerController>().player;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref currentVelocity, playerModel.smoothRotationTime);
        }
    }
    public void ClimbLadder(float speed)
    {
        if (photonView.IsMine)
        {
            playerModel = GetComponent<PlayerController>().player;
            transform.position = new Vector3(playerModel.posX, playerModel.posY, playerModel.posZ);
            transform.eulerAngles = new Vector3(playerModel.rotX, playerModel.rotY, playerModel.rotZ);
            anim.SetFloat("Speed", speed);
            anim.SetBool("climbingLadder", true);
        }
    }
    public IEnumerator MoveToSpawnPosition()
    {
        if (photonView.IsMine)
        {
            playerModel = GetComponent<PlayerController>().player;
            float time = 10;
            while (time >= 0)
            {
                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
            }
            anim.applyRootMotion = false;
            anim.rootPosition = new Vector3(playerModel.posX, playerModel.posY, playerModel.posZ);
            transform.position = anim.rootPosition;
            anim.applyRootMotion = true;
            transform.eulerAngles = new Vector3(playerModel.rotX, playerModel.rotY, playerModel.rotZ);

        }

    }
    public void Jump()
    {
        if (photonView.IsMine)
        {
            playerModel = GetComponent<PlayerController>().player;
            anim.SetTrigger("jump");
            inGameSoundsManager.JumpSound();
            float jumpVelocity = Mathf.Sqrt(-2 * playerModel.gravity * playerModel.JumpForce);
            playerModel.posY = jumpVelocity;
        }
    }

    public void Slide()
    {
        if (photonView.IsMine)
        {
            anim.SetTrigger("Slide");
            Invoke("ResetSlideTrigger", 1f);
        }
    }
    public void ResetSlideTrigger()
    {
        anim.ResetTrigger("Slide");
    }
    public void Crouch()
    {
        if (photonView.IsMine)
        {
            if (anim.GetBool("crouch") == false)
                anim.SetBool("crouch", true);
            else if (anim.GetBool("crouch") == true)
                anim.SetBool("crouch", false);
        }

    }
    public void Health()
    {
        if (photonView.IsMine)
        {
            playerModel = GetComponent<PlayerController>().player;
            HealthfillImage.fillAmount = playerModel.health;
        }
    }
    [PunRPC]
    public void Death()
    {
        anim.Play("Death");
        characterController.detectCollisions = false;
        if (photonView.IsMine)
        {
            CameraModel cameramodel = cameracontroller.cameraModel;
            cameramodel.deathcam = true;
        }
            Invoke("RespawnPlayer", 10f);
    }
    private void RespawnPlayer()
    {
        //playerHips.SetActive(true);
        //playerMeshes.SetActive(true);
        anim.Play("Idle");
        characterController.detectCollisions = true;
        if (photonView.IsMine)
        {
            CameraModel cameramodel = cameracontroller.cameraModel;
            cameramodel.deathcam = false;
            Health();
        }
    }
    public void SetMostKills(int _kills)
    {
        if (photonView.IsMine)
        {
            mostKillsTxt.text = _kills + "";
        }
    }
    public void SetYouKills(int _kills)
    {
        if (photonView.IsMine)
        {
            youKillsTxt.text = _kills + "";
        }
    }
    public void SetWinner()
    {
        if (photonView.IsMine)
        {
            matchResultFrameBar.SetActive(true);
            victoryImage.SetActive(true);
            crosshairBar.SetActive(false);
        }
    }
    public void SetLoser()
    {
        if (photonView.IsMine)
        {
            matchResultFrameBar.SetActive(true);
            defeatImage.SetActive(true);
            crosshairBar.SetActive(false);

        }
    }
    public void ChatButton()
    {
        if (photonView.IsMine)
        {
            if(!chatMessages.activeSelf)
                chatMessages.SetActive(true);
            else
                chatMessages.SetActive(false);

        }
    }
    public void PauseButton()
    {
        Animator animator = pauseMenuBar.GetComponent<Animator>();
        CanvasGroup canvasgroup = pauseMenuBar.GetComponent<CanvasGroup>();
        canvasgroup.interactable = true;
        canvasgroup.blocksRaycasts = true;
        animator.SetTrigger("Start");
    }
    public void PlayButton()
    {
        Animator animator = pauseMenuBar.GetComponent<Animator>();
        CanvasGroup canvasgroup = pauseMenuBar.GetComponent<CanvasGroup>();
        canvasgroup.interactable = false;
        canvasgroup.blocksRaycasts = false;
        animator.SetTrigger("End");
    }
}
