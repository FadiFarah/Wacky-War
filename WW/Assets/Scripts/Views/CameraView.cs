using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviourPun
{
    CameraModel cameraModel;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    public PhotonView playerPhotonView;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Rotate()
    {
        if (playerPhotonView.IsMine)
        {
            cameraModel = GetComponent<CameraController>().cameraModel;
            cameraModel.rotX = Mathf.Clamp(cameraModel.rotX, cameraModel.pitchMin, cameraModel.pitchMax);
            currentRotation = new Vector3(cameraModel.rotX, cameraModel.rotY, cameraModel.rotZ);
            transform.eulerAngles = Vector3.SmoothDamp(currentRotation, new Vector3(cameraModel.rotX, cameraModel.rotY), ref rotationSmoothVelocity, cameraModel.rotationSmoothTime);
        }
    }
    public void Move()
    {
        if (playerPhotonView.IsMine)
        {
            cameraModel = GetComponent<CameraController>().cameraModel;
            transform.position = new Vector3(cameraModel.posX, cameraModel.posY, cameraModel.posZ) - transform.forward * cameraModel.distanceFromTarget;
        }
    }
    public void DeathCam()
    {
        if (playerPhotonView.IsMine)
        {
            cameraModel = GetComponent<CameraController>().cameraModel;
            transform.parent = null;
            transform.position = Vector3.Lerp(transform.position, GameObject.Find("DeathView").transform.position, 1.5f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, GameObject.Find("DeathView").transform.rotation, 1.5f * Time.deltaTime);
            Invoke("DeathCamEnded", 10);
        }
    }
    public void DeathCamEnded()
    {
        if(playerPhotonView.IsMine)
        {
            transform.parent = playerPhotonView.transform;
        }
    }

    public GameObject LocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
                return p;
            }
        }
        return null;
    }

}
