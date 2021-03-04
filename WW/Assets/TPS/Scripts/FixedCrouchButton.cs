using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class FixedCrouchButton : MonoBehaviour, IPointerClickHandler
{
    public MyPlayer player;
    public void SetPlayer(MyPlayer _player)
    {
        player = _player;

    }
    public void OnPointerClick(PointerEventData eventData)
    {

        player.Crouch();

    }
}
