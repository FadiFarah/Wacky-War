using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class FixedShootButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    public Snowball_Shoot player;
    public void SetPlayer(Snowball_Shoot _player)
    {
        player = _player;

    }
    public void OnPointerClick(PointerEventData eventData)
    {
            player.shooting = true;
            player.Shoot();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
            player.shooting = false;
    }
}
