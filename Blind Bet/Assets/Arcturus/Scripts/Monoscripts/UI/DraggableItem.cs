using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public Image image;
    public Card card;
    public void OnBeginDrag(PointerEventData eventData)
    {
        MenuField menuField = GetComponentInParent<MenuField>();
        if(menuField != null)
        {
            if (menuField)
            {
                menuField.InvokeOnExit(new MenuField.Field(card, menuField.handNum));
            }
        }
        

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        transform.localScale = transform.localScale*1.1f;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        for(int i = 0; i < parentAfterDrag.childCount; i++)
        {
            DraggableItem childCard = parentAfterDrag.GetChild(i).GetComponent<DraggableItem>();
            if(childCard.card.rank > card.rank)
            {
                transform.SetSiblingIndex(i);
                break;
            }
        }
        image.raycastTarget = true;
        transform.localScale = transform.localScale/1.1f;
    }
}
