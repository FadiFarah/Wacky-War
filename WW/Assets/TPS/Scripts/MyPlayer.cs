using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MyPlayer : MonoBehaviourPun
{
    public float MoveSpeed = 30f;
    public float gravity = -50;
    public float smoothRotationTime = 0.25f;
    public bool enableMobileInputs = false;
    public GameObject crossHairPrefab;
    float currentVelocity;
    float velocityY;
    float currentSpeed;
    float speedVelocity;
    public float JumpForce;

    public bool isgrounded;
    public bool iscrouching;
    public bool issliding;

    //sounds
    public List<AudioSource> footsteps;
    int num = 0;

    //Health
    public Image HealthfillImage;
    public float playerHealth=1;
    public GameObject healthBar;

    //chatSystem
    public GameObject chatSystem;

    //teams
    public GameObject teamText;

    private CharacterController characterController;
    private GameObject SnowBall;
    private Animator anim;
    private FixedJoystick joystick;
    public Camera secondaryCam;

    private bool throwsnow;


    void Awake()
    {
        if(photonView.IsMine)
        { 
            joystick = GameObject.Find("Joystick").GetComponent<FixedJoystick>();
            iscrouching = false;
            characterController = GetComponent<CharacterController>();
            chatSystem.SetActive(true);
            teamText.SetActive(true);
            teamText.GetComponent<Text>().text = "Team : "+PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        }
    }
    void Start()
    {
        
        if (photonView.IsMine)
        {
            anim = GetComponent<Animator>();
            GameObject.Find("JumpButton").GetComponent<FixedJumpButton>().SetPlayer(this);
            GameObject.Find("SlideButton").GetComponent<FixedSlideButton>().SetPlayer(this);
            GameObject.Find("CrouchButton").GetComponent<FixedCrouchButton>().SetPlayer(this);
            crossHairPrefab = Instantiate(crossHairPrefab);
            isgrounded = characterController.isGrounded;
            healthBar.SetActive(true);
        }

    }
    void Update()
    {
        
        if (photonView.IsMine)
        {
            LocalPlayerUpdate();
        }
    }
    void LocalPlayerUpdate()
    {
        anim.SetBool("Grounded", characterController.isGrounded);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        Vector2 input = Vector2.zero;
        if (enableMobileInputs)
        {
            //if Horizontal is 1 it will move right -1 left(0,0,1) if vertical 1 forward -1 backwards (1,0,0)
            input = new Vector2(joystick.input.x, joystick.input.y);
        }
        else
        {
            //if Horizontal is 1 it will move right -1 left(0,0,1) if vertical 1 forward -1 backwards (1,0,0)
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        Vector2 inputDir = input.normalized;//To keep the values normalized between -1 and 1.

        //only if he is moving change the angle direction
        //Vector2.zero returns x = 0 y = 0
        if (inputDir != Vector2.zero)
        {
            //eulerAngles is used to transform the angle of the player.
            //Math.Atan2 returns the angle in radians, so we multiply it by Mathf.Rad2Deg and it will be converted to degrees. (if x is 1 and y is 1, it will rotate 45 degree)
            //Mathf.SmootDampAngle is used for smoothness rotation.
            //+cameraTransform.euelerAngles.y means to rotate the player as the camera's y rotation.
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref currentVelocity, smoothRotationTime);
            if (!footsteps[0].isPlaying)
            {
                footsteps[0].Play();
                num = 1;
            }
            if (!footsteps[1].isPlaying)
            {
                footsteps[1].Play();
                num = 0;
            }
        }
        else
        {
            foreach (var i in footsteps)
            {
                i.Stop();
            }
        }
        //Translate is used for the movement of the player.
        //.forward means to move the player forward depends on his character angle face direction.
        //multiplying by magnitude, means incase he is not moving (5f*magnitude will return 0)
        //multiplying by deltaTime for smoothness purposes.
        //Space.World means to change the x y and z axis as the player moves.
        //Space.Self means the x y and z axis will not change
        float targetSpeed = MoveSpeed * inputDir.magnitude;
        //Mathf.SmoothDamp is used for smoothness movement.
        //To go from the currentspeed to the targetspeed by 0.1f amount of time.
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, 0.1f);

        //transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        velocityY += Time.deltaTime * gravity;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        characterController.Move(velocity * Time.deltaTime);
        //currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
        currentSpeed = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;
        if (characterController.isGrounded)
        {
            velocityY = 0;
        }
        if (currentSpeed <= 0.8f)
        {
            footsteps[0].pitch = 0.40f;
            footsteps[1].pitch = 0.40f;
            anim.SetFloat("Speed", currentSpeed);
        }
        else if (currentSpeed > 0.8f)
        {
            footsteps[0].pitch = 1f;
            footsteps[1].pitch = 1f;
            anim.SetFloat("Speed", currentSpeed);
        }
    }
    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            PositionCrossHair();
        }
    }
    void PositionCrossHair()
    {
        crossHairPrefab.transform.LookAt(secondaryCam.transform);
    }
    public void Jump()
    {
        if (characterController.isGrounded)
        {
            anim.SetTrigger("jump");
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * JumpForce);
            velocityY = jumpVelocity;
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

    [PunRPC]
    public void GetDamage(float amount)
    {

            playerHealth -= amount;
        if (playerHealth <= 0 && photonView.IsMine)
        {
            Death();
        }
        if(photonView.IsMine)
            HealthfillImage.fillAmount = playerHealth;
    }
    [PunRPC]
    public void Hideplayer()
    {
        if (photonView.IsMine)
        {
            GameObject.Find("SecondaryCam").GetComponent<MyCamera>().enabled = false;
            GameObject.Find("Main Camera").GetComponent<MyCamera>().enabled = false;
            GameObject.Find("Main Camera").GetComponent<DeathCam>().enabled = true;
        }
        gameObject.SetActive(false);
           Invoke("RespawnPlayer", 10f);
            
    }
    public void RespawnPlayer()
    {
        gameObject.SetActive(true);
        playerHealth = 1;
        if (photonView.IsMine)
        {
            HealthfillImage.fillAmount = playerHealth;
            gameObject.GetComponent<Snowball_Shoot>().bulletsLeft = gameObject.GetComponent<Snowball_Shoot>().magazineSize;
            GameObject.Find("Main Camera").GetComponent<DeathCam>().enabled = false;
            GameObject.Find("SecondaryCam").GetComponent<MyCamera>().enabled = true;
            GameObject.Find("Main Camera").GetComponent<MyCamera>().enabled = true;
        }
    }
    void Death()
    {
        anim.SetTrigger("death");
        photonView.RPC("Hideplayer",RpcTarget.All);
    }

    
}