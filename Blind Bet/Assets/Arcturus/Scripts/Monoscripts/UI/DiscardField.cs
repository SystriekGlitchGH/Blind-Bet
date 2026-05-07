using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiscardField : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Destroy(dropped.gameObject);
    }
}
