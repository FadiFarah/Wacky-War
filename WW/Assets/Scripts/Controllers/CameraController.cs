using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviourPun
{


    public CameraModel cameraModel;
    CameraView cameraView;
    Transform cameraTransform;
    public PhotonView playerPhotonView;

    public GameObject touchFieldBar;
    FixedTouchField touchField;


    void Start()
    {
        if (playerPhotonView.IsMine)
        {
            touchFieldBar.SetActive(true);
            touchField = GameObject.Find("TouchField").GetComponent<FixedTouchField>();
            cameraTransform = LocalPlayer().transform.GetChild(2);
            cameraModel = new CameraModel(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z, cameraTransform.rotation.x, cameraTransform.rotation.y, cameraTransform.rotation.z);
            cameraView = GetComponent<CameraView>();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerPhotonView.IsMine)
        {
        if (touchField.Pressed)
        {
            cameraModel.rotY += touchField.TouchDist.x * cameraModel.RotationSensitivity;
            cameraModel.rotX -= touchField.TouchDist.y * cameraModel.RotationSensitivity;
            cameraModel.rotX = Mathf.Clamp(cameraModel.rotX, cameraModel.pitchMin, cameraModel.pitchMax);
        }
            cameraModel.SetRotation(cameraModel.rotX, cameraModel.rotY,cameraModel.rotZ);
            cameraModel.SetPosition(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z);
            cameraView.Rotate();
            cameraView.Move();
        }
    }
    public GameObject LocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject p in players)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
                return p;
            }
        }
        return null;
    }
}
