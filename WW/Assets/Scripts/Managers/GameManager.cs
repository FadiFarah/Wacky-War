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
    //public Camera sceneCam;
    //public GameObject player;
    public List<GameObject> availablePlayerSpawnPositions;
    public List<GameObject> unavailablePlayerSpawnPositions;
    GameObject currentSpawnPosition;
    public int spawnIndex;
    //public Text pingRateText;
    public float matchTime;
    public TMP_Text timeRemainTxt;
    public int maxkills;
    public bool timeEnded = false;
    public bool maxReached = false;
    public int max = 0;
    public int maxindex = 0;
    public int counter = 0;
    public GameView gameView;
    public GameObject ladder1Position;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 25; //20 default
        PhotonNetwork.SerializationRate = 10;
        //sceneCam.enabled = false;
        maxkills = 4;
        UIManager.instance.Invoke("ClearScreen", 3);
        gameView = GameObject.Find("GameMVC").GetComponent<GameView>();
        spawnIndex = ((PhotonNetwork.LocalPlayer.ActorNumber-1)%8);
        Debug.LogWarning(spawnIndex);
        currentSpawnPosition = GetSpawnPosition();
        PhotonNetwork.Instantiate("Player", currentSpawnPosition.transform.position, currentSpawnPosition.transform.rotation);
        StartCoroutine(EndMatch());

    }

    // Update is called once per frame
    void LateUpdate()
    {

        //pingRateText.text = PhotonNetwork.GetPing().ToString();
        if (timeEnded == false && maxReached == false)
        {
            GetFirstPlayerCurrentKills();
        }
    }
    private IEnumerator EndMatch()
    {
        float matchTime = 300;
        while (matchTime > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            matchTime -= Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(matchTime);

            string answer = string.Format("{0:D2}m:{1:D2}s ", t.Minutes, t.Seconds);
            if (PhotonNetwork.IsMasterClient)
                gameView.SendTime(answer);
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
            max = 0;
            maxindex = 0;
        }
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().kills > max)
            {
                max = players[i].GetComponent<PlayerController>().kills;
                maxindex = i;
                if (max == maxkills)
                {
                    maxReached = true;
                    timeEnded = true;
                }
            }
        }
        if (players.Length > 0)
            players[0].GetComponent<PhotonView>().RPC("SetMostKills", RpcTarget.All, max, maxReached, timeEnded);
    }
    public GameObject GetSpawnPosition()
    {
        gameView.SendMsg(GameView.EventCodes.spawnposition);
        unavailablePlayerSpawnPositions.Add(availablePlayerSpawnPositions[spawnIndex]);
        GameObject spawnPos = availablePlayerSpawnPositions[spawnIndex];
        availablePlayerSpawnPositions.RemoveAt(spawnIndex);
        Invoke("MakePositionAvailable", 5);
        return spawnPos;
    }
    public void MakePositionAvailable()
    {
        if(unavailablePlayerSpawnPositions.Count>0)
        {
            availablePlayerSpawnPositions.Add(unavailablePlayerSpawnPositions[0]);
            unavailablePlayerSpawnPositions.RemoveAt(0);
        }
    }

}
