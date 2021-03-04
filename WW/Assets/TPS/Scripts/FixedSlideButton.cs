using UnityEngine;
using UnityEngine.EventSystems;
public class FixedSlideButton : MonoBehaviour, IPointerClickHandler
{
    public MyPlayer player;
    public void SetPlayer(MyPlayer _player)
    {
        player = _player;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        player.Slide();
        player.issliding = true;
    }

}
