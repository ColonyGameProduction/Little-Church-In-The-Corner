using UnityEngine;
using UnityEngine.EventSystems;

public class DisableRoomSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        TransitionManager.Instance.B_enableSwipe = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TransitionManager.Instance.B_enableSwipe = true;
    }

    private void OnDisable()
    {
        TransitionManager.Instance.B_enableSwipe = true;
    }
}
