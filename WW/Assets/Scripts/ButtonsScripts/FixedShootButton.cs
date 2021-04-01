using UnityEngine;
using UnityEngine.EventSystems;

public class FixedShootButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler
{
    public SnowballController snowball;
    public void SetPlayer(SnowballController _snowball)
    {
        snowball = _snowball;

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        snowball.snowballModel.shooting = true;
        snowball.Shoot();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        snowball.snowballModel.shooting = true;
    }
}