using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera sceneCam;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        sceneCam.enabled = false;
        Instantiate(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
