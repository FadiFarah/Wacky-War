using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    public float posX = 0f;
    public float posY = 0f;
    public float posZ = 0f;
    public float rotX = 0f;
    public float rotY = 0f;
    public float rotZ = 0f;
    public int Kills = 0;
    public int Deaths = 0;
    public float health=1f;
    public float speed = 5f;
    public float gravity = -100f;
    public float smoothRotationTime = 0.25f;
    public float JumpForce = 10f;
    public bool climbingLadder = false;
    public bool zipLine = false;
    public void SetPosition(float posx, float posy, float posz)
    {
        posX = posx;
        posY = posy;
        posZ = posz;
    }
    public void SetRotation(float rotx, float roty, float rotz)
    {
        rotX = rotx;
        rotY = roty;
        rotZ = rotz;
    }
    public void SetKD(int kills,int deaths)
    {
        Kills = kills;
        Deaths = deaths;
    }

}
