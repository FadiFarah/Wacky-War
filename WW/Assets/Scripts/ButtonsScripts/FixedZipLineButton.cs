using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedZipLineButton : MonoBehaviour, IPointerClickHandler
{
    public PlayerController player;
    public void SetPlayer(PlayerController _player)
    {
        player = _player;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Zipping Click");
        player.player.zipLine = true;
    }
}
