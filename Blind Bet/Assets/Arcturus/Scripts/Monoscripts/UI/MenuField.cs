using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class MenuField : MonoBehaviour, IDropHandler
{
    [SerializeField] private UnityEvent<Card> _onCardEnter;
    public void InvokeOnEnter(Card card) => _onCardEnter?.Invoke(card);

    [SerializeField] private UnityEvent<Card> _onCardExit;
    public void InvokeOnExit(Card card) => _onCardExit?.Invoke(card);
    public int handNum;

    private void Awake()
    {
        foreach (var card in GetComponentsInChildren<DraggableItem>())
        {
            InvokeOnEnter(card.card);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem item = dropped.GetComponent<DraggableItem>();
        item.parentAfterDrag = transform;

        InvokeOnEnter(item.card);
    }
}
