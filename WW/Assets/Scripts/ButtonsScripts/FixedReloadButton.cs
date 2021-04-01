using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedReloadButton : MonoBehaviour, IPointerClickHandler
{
    public SnowballController player;
    public void SetPlayer(SnowballController _player)
    {
        player = _player;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        player.Reload();
    }


}