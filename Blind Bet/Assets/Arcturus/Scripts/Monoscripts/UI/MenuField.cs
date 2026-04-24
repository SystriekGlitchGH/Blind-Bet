using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class MenuField : MonoBehaviour, IDropHandler
{
    [SerializeField] private UnityEvent<Field> _onCardEnter;
    public void InvokeOnEnter(Field field) => _onCardEnter?.Invoke(field);

    [SerializeField] private UnityEvent<Field> _onCardExit;
    public void InvokeOnExit(Field field) => _onCardExit?.Invoke(field);
    [Serializable]
    public struct Field
    {
        public Card card;
        public int handNum;
        public Field(Card card, int handNum)
        {
            this.card = card;
            this.handNum = handNum;
        }
    }
    public int handNum;

    private void Awake()
    {
        foreach (var card in GetComponentsInChildren<DraggableItem>())
        {
            InvokeOnEnter(new Field(card.card,handNum));
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem item = dropped.GetComponent<DraggableItem>();
        item.parentAfterDrag = transform;

        InvokeOnEnter(new Field(item.card, handNum));
    }
}
