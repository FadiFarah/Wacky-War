using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviourPun
{


    public CameraModel cameraModel;
    CameraView cameraView;
    public Transform cameraTransform;
    public PhotonView playerPhotonView;

    public GameObject touchFieldBar;
    FixedTouchField touchField;
    Transform DeathCam;


    void Start()
    {
        if (playerPhotonView.IsMine)
        {
            touchFieldBar.SetActive(true);
            touchField = GameObject.Find("TouchField").GetComponent<FixedTouchField>();
            cameraModel = new CameraModel(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z, cameraTransform.rotation.x, cameraTransform.rotation.y, cameraTransform.rotation.z);
            cameraView = GetComponent<CameraView>();
            DeathCam = GameObject.Find("DeathView").GetComponent<Transform>();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPhotonView.IsMine)
        {
            if (cameraModel.deathcam==false)
            {
                if (touchField.Pressed)
                {
                    cameraModel.rotY += touchField.TouchDist.x * cameraModel.RotationSensitivity;
                    cameraModel.rotX -= touchField.TouchDist.y * cameraModel.RotationSensitivity;
                    cameraModel.rotX = Mathf.Clamp(cameraModel.rotX, cameraModel.pitchMin, cameraModel.pitchMax);
                    cameraModel.SetRotation(cameraModel.rotX, cameraModel.rotY, cameraModel.rotZ);
                }

                cameraModel.SetPosition(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z);
                cameraView.Rotate();
                cameraView.Move();
            }
            else
            {
                cameraModel.SetRotation(DeathCam.rotation.x, DeathCam.rotation.y, DeathCam.rotation.z);
                cameraModel.SetPosition(DeathCam.position.x, DeathCam.position.y, DeathCam.position.z);
                cameraView.DeathCam();
            }
        }
    }

}
