using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject playersContainer;
    public GameObject playerAvatarPrefab;

    public void AddPlayer(string playerName)
    {
       GameObject PaP= Instantiate(playerAvatarPrefab, Vector3.zero, Quaternion.identity);
        PaP.transform.GetChild(0).GetComponent<Text>().text = playerName;
        PaP.transform.parent = playersContainer.transform;
        PaP.transform.localScale = Vector3.one;
        PaP.name = playerName;
    }
    public void RemovePlayer(string playerName)
    {
        int PaPCount = playersContainer.transform.childCount;
        for(int i = 0; i < PaPCount; i++)
        {
            if (playersContainer.transform.GetChild(i).name == playerName)
            {
                Destroy(playersContainer.transform.GetChild(i).gameObject);
                return;
            }
        }
    }
}
