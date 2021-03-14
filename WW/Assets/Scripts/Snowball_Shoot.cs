using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class Snowball_Shoot : MonoBehaviourPun
{
    private Animator anim;
    public AudioSource shootSound;

    //bullet
    public GameObject bullet;
    public GameObject bulletInHand;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timebetweenShots;
    public int magazineSize, bulletsPerTab;
    public bool allowButtonHold;
    [HideInInspector]
    public int bulletsLeft, bulletsShot;

    //bools
    public bool shooting;
    bool readyToShoot, reloading;

    //Reference
    public Camera fpscam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunationDisplay;

    //Bug fixing purposes
    public bool allowInvoke = true;

    public bool enableMobileInputs;
    private void Awake()
    {
        //make sure magazine is full
        bulletsLeft = magazineSize;
        ammunationDisplay = GameObject.Find("AmmunationDisplay").GetComponent<TextMeshProUGUI>();
        readyToShoot = true;
        enableMobileInputs = gameObject.GetComponent<MyPlayer>().enableMobileInputs;
        if (photonView.IsMine)
        {
            GameObject.Find("ReloadButton").GetComponent<FixedReloadButton>().SetPlayer(this);
            GameObject.Find("ShootButton").GetComponent<FixedShootButton>().SetPlayer(this);
        }

    }
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            MyInput();
            //Set ammo display, if it exists
            if (ammunationDisplay != null)
            {
                ammunationDisplay.SetText(bulletsLeft / bulletsPerTab + " / " + magazineSize / bulletsPerTab);
            }
        }
    }
    private void MyInput()
    {
        if (!enableMobileInputs)
        {
            //Check if allowed to hold down button and take corresponding input
            if (allowButtonHold) shooting = Input.GetKey(KeyCode.CapsLock);
            else shooting = Input.GetKeyDown(KeyCode.CapsLock);
        }


        //Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot = 0;
            photonView.RPC("Shoot", RpcTarget.All);
        }
    }
    [PunRPC]
    public void Shoot()
    {
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            
            anim.SetTrigger("throw");
            bulletInHand.SetActive(false);
            shootSound.Play();
            readyToShoot = false;
            //Find the exact hit position using a raycast
            Ray ray = fpscam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            transform.rotation = new Quaternion(0, Camera.main.transform.rotation.y, 0, Camera.main.transform.rotation.w);
            //attackPoint.transform.eulerAngles = new Vector3(ray.direction.x, fpscam.transform.eulerAngles.y, ray.direction.z);

            //check if ray hits something
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(70); //Just a point far away from the player

            //Calculate direction from attackPoint to targetPoint
            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

            //Calculate spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            //Calculate new direction with spread;
            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just adds spread to last

            //Instantiate bullet/projectile
            if (photonView.IsMine)
            {
                GameObject currentBullet = PhotonNetwork.Instantiate(bullet.name, attackPoint.position, Quaternion.identity);
                //Rotate bullet to shoot direction
                currentBullet.transform.forward = directionWithSpread.normalized;
                //Add forces to bullet
                currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
                currentBullet.GetComponent<Rigidbody>().AddForce(fpscam.transform.up * upwardForce, ForceMode.Impulse);
            }
            //Instantiate muzzle flash, if you have one
            if (muzzleFlash != null)
            {
                Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            }


            bulletsLeft--;
            bulletsShot++;

            if (photonView.IsMine)
            {
                //Invoke resetShot function (if not already invoked)
                if (allowInvoke)
                {
                    Invoke("ResetShot", timeBetweenShooting);
                    allowInvoke = false;
                }

                //if more than one bulletsPerTab make sure to repeat shoot function
                if (bulletsShot < bulletsPerTab && bulletsLeft > 0)
                    Invoke("Shoot", timebetweenShots);
            }
        }
    }
    
    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
        bulletInHand.SetActive(true);
        shooting = false;
    }
    public void Reload()
    {
        reloading = true;
        anim.SetTrigger("reload");
        gameObject.GetComponent<MyPlayer>().isreloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        anim.ResetTrigger("reload");
        gameObject.GetComponent<MyPlayer>().isreloading = false;
        reloading = false;
        shooting = false;
    }
}
