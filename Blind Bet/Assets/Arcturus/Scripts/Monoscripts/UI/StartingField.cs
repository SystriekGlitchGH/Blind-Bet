using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

public class StartingField : MonoBehaviour, IDropHandler
{
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
    public CardDeck cardDeck;
    public bool isMainField;
    Random rand = new Random();

    private void Awake()
    {
        if (isMainField)
        {
            for(int i = 0; i < 5; i++)
            {
                // finding rank
                int rank = 0;
                rank = rand.Next(2,5);
                // finding suit
                Card.Suit suit = Card.Suit.blank;
                int suitNum = rand.Next(1,4);
                if(suitNum == 1)
                    suit = Card.Suit.diamond;
                else if(suitNum == 2)
                    suit = Card.Suit.club;
                else if(suitNum == 3)
                    suit = Card.Suit.spade;
                // adding to player
                Instantiate(cardDeck.GetUIObjectFromCard(cardDeck.GetCardFromComponents(rank,suit)),transform);
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
        }
    }
}
