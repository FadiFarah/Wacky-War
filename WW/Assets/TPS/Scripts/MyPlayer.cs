using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
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

    //sounds
    public List<AudioSource> footsteps;
    int num = 0;

    private CharacterController characterController;
    public Transform cameraTransform;
    private GameObject SnowBall;
    private Animator anim;
    public FixedJoystick joystick;
    public Camera secondaryCam;

    private bool throwsnow;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        //GameObject.Find("ThrowBtn").GetComponent<ThrowBtnScript>().SetPlayer(this);
        crossHairPrefab = Instantiate(crossHairPrefab);

    }
    void Update()
    {
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
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg+Camera.main.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y,rotation,ref currentVelocity,smoothRotationTime);
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
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up*velocityY;
        characterController.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
        if (characterController.isGrounded)
        {
            velocityY = 0;
        }
        anim.SetFloat("Speed", currentSpeed/30);
    }
    private void LateUpdate()
    {
        PositionCrossHair();
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
}