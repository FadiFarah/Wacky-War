using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, GameObject.Find("SceneCamera").transform.position, 1.5f* Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, GameObject.Find("SceneCamera").transform.rotation, 1.5f*Time.deltaTime);
    }
}
