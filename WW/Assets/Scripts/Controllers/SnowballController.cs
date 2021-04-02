using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnowballController : MonoBehaviour
{
    public SnowballModel snowballModel;
    public PhotonView playerPhotonView;
    SnowballView snowballView;

    public GameObject shootBar;
    public GameObject reloadBar;


    //Graphics
    public GameObject ammunationBar;
    TextMeshProUGUI ammunationDisplay;
    // Start is called before the first frame update
    void Start()
    {
        if (playerPhotonView.IsMine)
        {
            shootBar.SetActive(true);
            ammunationBar.SetActive(true);
            reloadBar.SetActive(true);
            GameObject.Find("Shoot_Btn").GetComponent<FixedShootButton>().SetPlayer(this);
            GameObject.Find("Reload_Btn").GetComponent<FixedReloadButton>().SetPlayer(this);
            ammunationDisplay = GameObject.Find("AmmunationDisplay").GetComponent<TextMeshProUGUI>();
            snowballModel = new SnowballModel();
            snowballView = GetComponent<SnowballView>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPhotonView.IsMine)
        {
            MyInput();
            if (ammunationDisplay != null)
            {
                ammunationDisplay.SetText(snowballModel.bulletsLeft / snowballModel.bulletsPerTab + " / " + snowballModel.magazineSize / snowballModel.bulletsPerTab);
            }
        }
    }
    public void MyInput()
    {
        if (playerPhotonView.IsMine)
        {
            if (snowballModel.readyToShoot && snowballModel.shooting && !snowballModel.reloading && snowballModel.bulletsLeft <= 0) Reload();

            //Shooting
            if (snowballModel.readyToShoot && snowballModel.shooting && !snowballModel.reloading && snowballModel.bulletsLeft > 0)
            {
                //Set bullets shot to 0
                snowballModel.bulletsShot = 0;
                Shoot();
            }
        }
    }
    public void Shoot()
    {
        if (playerPhotonView.IsMine)
        {
            if (snowballModel.readyToShoot && snowballModel.shooting && !snowballModel.reloading && snowballModel.bulletsLeft > 0)
            {

                //anim.SetTrigger("throw");
                //bulletInHand.SetActive(false);
                // shootSound.Play();
                snowballView.Shoot();

                snowballModel.readyToShoot = false;
                snowballModel.bulletsLeft--;
                snowballModel.bulletsShot++;


                    //Invoke resetShot function (if not already invoked)
                    if (snowballModel.allowInvoke)
                    {
                        Invoke("ResetShot", snowballModel.timeBetweenShooting);
                        snowballModel.allowInvoke = false;
                    }

                    //if more than one bulletsPerTab make sure to repeat shoot function
                    if (snowballModel.bulletsShot < snowballModel.bulletsPerTab && snowballModel.bulletsLeft > 0)
                        Invoke("Shoot", snowballModel.timebetweenShots);
           
            }
        }
    }

    private void ResetShot()
    {
        //Allow shooting and invoking again
        if (playerPhotonView.IsMine)
        {
            snowballModel.readyToShoot = true;
            snowballModel.allowInvoke = true;
            snowballModel.shooting = false;
            snowballView.ResetShot();
        }
    }
    public void Reload()
    {
        if (playerPhotonView.IsMine && snowballModel.reloading==false && snowballModel.bulletsLeft<10)
        {
            snowballView.Reload();
            snowballModel.reloading = true;
            Invoke("ReloadFinished", snowballModel.reloadTime);
        }
    }

    private void ReloadFinished()
    {
        if(playerPhotonView.IsMine)
        { 
            snowballView.ReloadFinished();
            snowballModel.bulletsLeft = 10;
            snowballModel.magazineSize--;
            snowballModel.reloading = false;
            snowballModel.shooting = false;
        }
    }
}
