using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleButton : MonoBehaviour,IBeginDragHandler, IDragHandler
{
    private ToggleController toggle;

    public void Start()
    {
        toggle = transform.parent.GetComponent<ToggleController>();
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        toggle.Switching();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}
