using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel
{
    public float posX { get; set; }
    public float posY { get; set; }
    public float posZ { get; set; }
    public float rotX { get; set; }
    public float rotY { get; set; }
    public float rotZ { get; set; }
    public float distanceFromTarget = 40;
    public float RotationSensitivity = 1f;
    public float rotationSmoothTime = 400f;
    public float pitchMin = -70f;
    public float pitchMax = 70f;
    public CameraModel(float posx, float posy, float posz, float rotx, float roty, float rotz)
    {
        posX = posx;
        posY = posy;
        posZ = posz;
        rotX = rotx;
        rotY = roty;
        rotZ = rotz;
    }
    public void SetPosition(float posX,float posY,float posZ)
    {
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
    }
    public void SetRotation(float rotX,float rotY,float rotZ)
    {
        this.rotX = rotX;
        this.rotY = rotY;
        this.rotZ = rotZ;
    }

}
