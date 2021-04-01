using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballModel
{
    //bullet force
    public float shootForce = 120f;
    public float upwardForce=5f;

    //Gun stats
    public float timeBetweenShooting=1f;
    public float spread=1;
    public float reloadTime=1.5f;
    public float timebetweenShots=1f;
    public int magazineSize=10, bulletsPerTab=1;
    public bool allowButtonHold=false;
    public int bulletsLeft=10, bulletsShot=0;

    //bools
    public bool shooting=false;
    public bool readyToShoot=true;
    public bool reloading=false;

    //Bug fixing purposes
    public bool allowInvoke = true;

}
