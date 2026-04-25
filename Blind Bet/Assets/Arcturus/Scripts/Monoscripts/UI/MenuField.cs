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
    public CardDeck cardDeck;
    private void Awake()
    {
        Player player = FindFirstObjectByType<PlayerMovement>().playerStats;
        if(handNum == 1)
        {
            for(int i = 0; i < player.activeHand.cards.Length; i++)
            {
                if(player.activeHand.cards[i] != player.blankCard)
                {
                    Instantiate(cardDeck.GetUIObjectFromCard(player.activeHand.cards[i]),transform);
                }
            }
        }
        if(handNum == 2)
        {
            for(int i = 0; i < player.passiveHand1.cards.Length; i++)
            {
                if(player.passiveHand1.cards[i] != player.blankCard)
                {
                    Instantiate(cardDeck.GetUIObjectFromCard(player.passiveHand1.cards[i]),transform);
                }
            }
        }
        if(handNum == 3)
        {
            for(int i = 0; i < player.passiveHand2.cards.Length; i++)
            {
                if(player.passiveHand2.cards[i] != player.blankCard)
                {
                    Instantiate(cardDeck.GetUIObjectFromCard(player.passiveHand2.cards[i]),transform);
                }
            }
        }
        if(handNum == 4)
        {
            for(int i = 0; i < player.bench.Count; i++)
            {
                if(player.bench[i] != player.blankCard)
                {
                    Instantiate(cardDeck.GetUIObjectFromCard(player.bench[i]),transform);
                }
            }
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
