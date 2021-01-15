using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private Animator anim;
        
        public float SpeedGrimpe=1000f;
        public bool FirstLadder = false;
        public bool SecondLadder = false;
        public bool PlayerRopeSlide;
        public bool crouch;
        public bool m_Jump;
        public bool m_switch;
        public bool fire;
        public float Hinput;
        public GameObject ropeslidebutton;
        public GameObject ropeslidebg;
        public float Vinput;// the world-relative desired move direction, calculated from the camForward and user input.
        private AudioSource m_audio;

        private void Start()
        {
            m_audio = GetComponent<AudioSource>();
            anim = GetComponent<Animator>();
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
           
            if (!crouch)
            {
                crouch = CrossPlatformInputManager.GetButtonDown("Crouch");
            }
           else if (!fire)
           {
                fire = CrossPlatformInputManager.GetButtonDown("Fire1");
            }
            if(FirstLadder)
            {
                //transform.Translate(Vector3.up * SpeedGrimpe * Time.deltaTime);
                transform.position = new Vector3(25, transform.position.y+ SpeedGrimpe * Time.deltaTime*1.2f, 190);
                transform.rotation = new Quaternion(1.691f, -48.747f, 0,66.747f);
            }
           else if (SecondLadder)
            {
                //transform.Translate(Vector3.up * SpeedGrimpe * Time.deltaTime);
                transform.position = new Vector3(-81, transform.position.y + SpeedGrimpe * Time.deltaTime*1.2f, 186);
                transform.rotation = new Quaternion(1.691f, -3.747f, 0, 30.747f);
            }
            if (PlayerRopeSlide && gameObject.GetComponent<Rigidbody>().useGravity==false)
            {
                transform.Translate(Vector3.forward * SpeedGrimpe * Time.deltaTime*6);
               
            }
        }
        void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.tag=="Echelle" && crouch!=true)
            {
                if (col.gameObject.name == "Trigger3")
                {
                    FirstLadder = true;
                }
                else 
                {
                    SecondLadder = true;
                }
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                transform.rotation = new Quaternion(col.transform.rotation.x, col.transform.rotation.y, col.transform.rotation.z, col.transform.rotation.w);
                anim.SetBool("Escalade", true);
            }
            if (col.gameObject.tag == "Echelle2" && crouch != true)
            {
                FirstLadder = false;
                SecondLadder = false;
                anim.SetBool("Escalade", false);
                if (gameObject.GetComponent<Rigidbody>().useGravity == false && col.gameObject.name=="Trigger2")
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y+5, transform.position.z + 10);
                    transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                }
                else if (gameObject.GetComponent<Rigidbody>().useGravity == false && col.gameObject.name == "Trigger4")
                {
                    transform.position = new Vector3(transform.position.x-10, transform.position.y+5, transform.position.z);
                    transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                }
                gameObject.GetComponent<Rigidbody>().useGravity = true;
               
            }
            if(col.gameObject.tag=="RopeT")
            {
                ropeslidebutton.SetActive(true);
                ropeslidebg.SetActive(true);
                
            }
            if(col.gameObject.tag=="RopeT2")
            {
                PlayerRopeSlide = false;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                m_Character.m_MovingTurnSpeed = 360;
                m_Character.m_StationaryTurnSpeed = 180;
                m_Character.m_MoveSpeedMultiplier = 1.2f;
                anim.SetBool("Sliding", false);
            }
        }
        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.tag == "RopeT" || gameObject.name=="ty")
            {
                ropeslidebutton.SetActive(false);
                ropeslidebg.SetActive(false);
            }
        }

        public void SlideOnRope(Collider col)
        {
            PlayerRopeSlide = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            m_Character.m_MovingTurnSpeed = 0;
            m_Character.m_StationaryTurnSpeed = 0;
            m_Character.m_MoveSpeedMultiplier = 0;
            if (gameObject.GetComponent<Rigidbody>().useGravity == false)
            {
                transform.rotation = new Quaternion(col.transform.rotation.x+5, 80.15f, col.transform.rotation.z, 80.15f);
                transform.position = new Vector3(col.transform.position.x + 10, col.transform.position.y -10, transform.position.z);
            }
            anim.SetBool("Sliding", true);
        }

       


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
           // float v = CrossPlatformInputManager.GetAxis("Vertical");
           //crouch = CrossPlatformInputManager.GetButtonDown("Crouch");

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                    // calculate camera relative direction to move:
                    m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = Vinput * m_CamForward + Hinput * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = Vinput * Vector3.forward + Hinput * Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
