using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedReloadButton : MonoBehaviour, IPointerClickHandler
{
    public Snowball_Shoot player;
    public void SetPlayer(Snowball_Shoot _player)
    {
        player = _player;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        player.Reload();
    }


}
