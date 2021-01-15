using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public float Yaxis;
    public float Xaxis;
    public bool enableMobileInputs=false;

    float RotationSensitivity = 8f;
    float RotationMin=-24.5f;
    float RotationMax=80f;
    float smoothTime = 0.1f;

    public Transform target;
    Vector3 targetRotation;
    Vector3 currentVel;
    public FixedTouchField touchField;
    public float offset;

    private void Start()
    {
        if (enableMobileInputs)
            RotationSensitivity = 0.2f;
    }
    void LateUpdate()
    {
        if (enableMobileInputs)
        {
            //If we go Horizontally with the mouse, we have to rotate the camera on the Y axis.
            Yaxis += touchField.TouchDist.x * RotationSensitivity;
            //If we go Vertically with the mouse, we have to rotate the camera on the X axis.
            Xaxis -= touchField.TouchDist.y * RotationSensitivity;
        }
        else
        {
            //If we go Horizontally with the mouse, we have to rotate the camera on the Y axis.
            Yaxis += Input.GetAxis("Mouse X") * RotationSensitivity;
            //If we go Vertically with the mouse, we have to rotate the camera on the X axis.
            Xaxis -= Input.GetAxis("Mouse Y") * RotationSensitivity;
        }
        //Restricting the transformation of Xaxis of the camera to a min and max. Means the rotation look is limited
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        //Variable of Vector3 for the rotations of X and Y Axis of the camera in a smooth way
        //Vector3 targetRotation = new Vector3(Xaxis, Yaxis);
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);

        //Change the rotation of the camera
        transform.eulerAngles = targetRotation;

        //Make the camera follow the player
        //Keep the camera's position away from the target player by 2 units.
        //transform.position = target.position - transform.forward * 30f;
        Vector3 _offset = target.position- transform.forward * offset;
        _offset.y = 15f+GameObject.Find("Player").transform.position.y;
        transform.position = _offset;
    }

}