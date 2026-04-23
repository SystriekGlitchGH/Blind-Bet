using UnityEngine;
using UnityEngine.EventSystems;
public class MenuField : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem item = dropped.GetComponent<DraggableItem>();
        item.parentAfterDrag = transform;
    }
}
