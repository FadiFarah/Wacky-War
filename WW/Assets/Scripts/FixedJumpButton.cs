﻿using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJumpButton : MonoBehaviour, IPointerClickHandler
{
    public MyPlayer player;
    public void SetPlayer(MyPlayer _player)
    {
        player = _player;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        player.Jump();
    }
}
