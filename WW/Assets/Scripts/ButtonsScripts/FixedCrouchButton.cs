using UnityEngine;
using UnityEngine.EventSystems;

public class FixedCrouchButton : MonoBehaviour, IPointerClickHandler
{
    public PlayerView player;
    public void SetPlayer(PlayerView _player)
    {
        player = _player;

    }
    public void OnPointerClick(PointerEventData eventData)
    {

        player.Crouch();

    }
}
