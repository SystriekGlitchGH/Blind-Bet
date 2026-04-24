using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    public GridLayoutGroup gridLayoutGroup;
    public int handNum;
    private void Awake()
    {
        //Player player = FindFirstObjectByType<PlayerMovement>().playerStats;
        //foreach (var card in player.bench)
        //{

        //}

        foreach (var card in GetComponentsInChildren<DraggableItem>())
        {
            InvokeOnEnter(new Field(card.card,handNum));
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount < gridLayoutGroup.constraintCount)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem item = dropped.GetComponent<DraggableItem>();
            item.parentAfterDrag = transform;
            
            InvokeOnEnter(new Field(item.card, handNum));
        }
        
    }
}
