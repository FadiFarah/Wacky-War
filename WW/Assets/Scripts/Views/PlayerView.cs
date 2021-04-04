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
    public void Jump()
    {
        if (photonView.IsMine)
        {
            playerModel = GetComponent<PlayerController>().player;
            anim.SetTrigger("jump");
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
            HealthfillImage.fillAmount = playerModel.health;
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
