using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;
using System;

public class GameManager : MonoBehaviourPun
{
    public Camera sceneCam;
    //public GameObject player;
    //public Transform playerSpawnPosition;
    //public Text pingRateText;
    public float matchTime;
    public TMP_Text timeRemainTxt;
    public int maxkills;
    public bool timeEnded = false;
    public bool maxReached = false;
    public int max = 0;
    public int maxindex = 0;
    public int counter = 0;


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 25; //20 default
        PhotonNetwork.SerializationRate = 10;
        sceneCam.enabled = false;
        maxkills = 4;
        //PhotonNetwork.Instantiate(player.name, playerSpawnPosition.position, playerSpawnPosition.rotation);
        PhotonNetwork.Instantiate("Player", Vector3.one, Quaternion.identity);
        StartCoroutine(EndMatch());
    }

    // Update is called once per frame
    void Update()
    {
        //pingRateText.text = PhotonNetwork.GetPing().ToString();
        if (timeEnded == false && maxReached == false)
        {
            Invoke("GetFirstPlayerCurrentKills",0.2f);
        }
    }
    private IEnumerator EndMatch()
    {
        float matchTime = 15f;
        while (matchTime > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            matchTime -= Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(matchTime);

            string answer = string.Format("{0:D2}m:{1:D2}s ", t.Minutes, t.Seconds);
            timeRemainTxt.text = answer;
        }
        timeEnded = true;
        maxReached = true;
        GetFirstPlayerCurrentKills();
    }
    private void GetFirstPlayerCurrentKills()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            max = players[0].GetComponent<PlayerController>().player.Kills;
            maxindex = 0;
        }
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().player.Kills > max)
            {
                max = players[i].GetComponent<PlayerController>().player.Kills;
                maxindex = i;
                if (max == maxkills)
                {
                    maxReached = true;
                    timeEnded = true;
                }
            }
        }
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PhotonView>().RPC("SetMostKills", RpcTarget.AllBuffered, max, maxReached, timeEnded);
        }


    }

}
