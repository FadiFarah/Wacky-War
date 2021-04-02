using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnowballView : MonoBehaviourPun
{
    SnowballModel snowballModel;

    //bullet
    public GameObject bullet;
    public GameObject bulletInHand;

    //Reference
    public Camera camera;
    public Transform attackPoint;

    private Animator anim;
    public PlayerView playerView;
    public PhotonView playerPhotonView;

    // Start is called before the first frame update
    void Start()
    {
        if(playerPhotonView.IsMine)
        {
            snowballModel = GetComponent<SnowballController>().snowballModel;
            playerView = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerView>();
            anim = playerView.GetComponent<Animator>();
        }
    }

    public void Shoot()
    {
        if (playerPhotonView.IsMine)
        {
            snowballModel = GetComponent<SnowballController>().snowballModel;
            anim.SetTrigger("throw");
            bulletInHand.SetActive(false);
            // shootSound.Play();
        }
            //Find the exact hit position using a raycast
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.6f, 0)); 
            RaycastHit hit;
            playerView.transform.rotation = new Quaternion(0, Camera.main.transform.rotation.y, 0, Camera.main.transform.rotation.w);
            //attackPoint.transform.eulerAngles = new Vector3(ray.direction.x, fpscam.transform.eulerAngles.y, ray.direction.z);

            //check if ray hits something
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(180); //Just a point far away from the player

            //Calculate direction from attackPoint to targetPoint
            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

            //Calculate spread
            float x = Random.Range(-snowballModel.spread, snowballModel.spread);
            float y = Random.Range(-snowballModel.spread, snowballModel.spread);

            //Calculate new direction with spread;
            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just adds spread to last

            //Instantiate bullet/projectile

            GameObject currentBullet = PhotonNetwork.Instantiate(bullet.name, attackPoint.position, Quaternion.identity);
            //Rotate bullet to shoot direction
            currentBullet.transform.forward = directionWithSpread.normalized;
            //Add forces to bullet
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * snowballModel.shootForce, ForceMode.Impulse);
            currentBullet.GetComponent<Rigidbody>().AddForce(camera.transform.up * snowballModel.upwardForce, ForceMode.Impulse);

            //Instantiate muzzle flash, if you have one
    }

    public void ResetShot()
    {
        if(playerPhotonView.IsMine)
        { 
            bulletInHand.SetActive(true);
        }
    }
    public void Reload()
    {
        if (playerPhotonView.IsMine)
        {
            anim.SetTrigger("reload");
        }
    }
    public void ReloadFinished()
    {
        if (playerPhotonView.IsMine)
        {
            anim.ResetTrigger("reload");
        }
    }
}
