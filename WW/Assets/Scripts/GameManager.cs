using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPun
{
    public Camera sceneCam;
    public GameObject player;
    public Transform playerSpawnPosition;
    public Text pingRateText;
    public float matchTime=10f;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 25; //20 default
        PhotonNetwork.SerializationRate = 10;
        sceneCam.enabled = false;
        PhotonNetwork.Instantiate(player.name, playerSpawnPosition.position, playerSpawnPosition.rotation);
        StartCoroutine(EndMatch());
    }

    // Update is called once per frame
    void Update()
    {
        pingRateText.text = PhotonNetwork.GetPing().ToString();
    }
    private IEnumerator EndMatch()
    {
       float matchTime = 10f;
        while (matchTime > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            matchTime -= Time.deltaTime;
        }
        Debug.Log("Match Ended");
    }
}
