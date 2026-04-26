using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeck", menuName = "Scriptable Objects/CardDeck")]
public class CardDeck : ScriptableObject
{
    [Header("Card Object Arrays")]
    public List<GameObject> diamondsObject;
    public List<GameObject> heartsObject;
    public List<GameObject> clubsObject;
    public List<GameObject> spadesObject;
    public GameObject blankCardObject;

    [Header("Card Arrays")]
    public List<Card> diamondsCard;
    public List<Card> heartsCard;
    public List<Card> clubsCard;
    public List<Card> spadesCard;
    public Card blankCardCard;

    public GameObject GetUIObjectFromCard(Card card)
    {
        if(card.suit == Card.Suit.diamond)
        {
            for(int i = 0; i < diamondsObject.Count; i++)
            {
                Card attachedCard = diamondsObject[i].GetComponent<DraggableItem>().card;
                if(attachedCard == card)
                    return diamondsObject[i];
            }
        }
        else if(card.suit == Card.Suit.heart)
        {
            for(int i = 0; i < heartsObject.Count; i++)
            {
                Card attachedCard = heartsObject[i].GetComponent<DraggableItem>().card;
                if(attachedCard == card)
                    return heartsObject[i];
            }
        }
        else if(card.suit == Card.Suit.club)
        {
            for(int i = 0; i < clubsObject.Count; i++)
            {
                Card attachedCard = clubsObject[i].GetComponent<DraggableItem>().card;
                if(attachedCard == card)
                    return clubsObject[i];
            }
        }
        else if(card.suit == Card.Suit.spade)
        {
            for(int i = 0; i < spadesObject.Count; i++)
            {
                Card attachedCard = spadesObject[i].GetComponent<DraggableItem>().card;
                if(attachedCard == card)
                    return spadesObject[i];
            }
        }
        return blankCardObject;
    }
    public Card GetCardFromComponents(int rank, Card.Suit suit)
    {
        if(suit == Card.Suit.diamond)
        {
            for(int i = 0; i < diamondsCard.Count; i++)
            {
                if(rank == i + 2)
                {
                    return diamondsCard[i];
                }
            }
        }
        else if(suit == Card.Suit.heart)
        {
            for(int i = 0; i < heartsCard.Count; i++)
            {
                if(rank == i + 2)
                {
                    return heartsCard[i];
                }
            }
        }
        else if(suit == Card.Suit.club)
        {
            for(int i = 0; i < clubsCard.Count; i++)
            {
                if(rank == i + 2)
                {
                    return clubsCard[i];
                }
            }
        }
        else if(suit == Card.Suit.spade)
        {
            for(int i = 0; i < spadesCard.Count; i++)
            {
                if(rank == i + 2)
                {
                    return spadesCard[i];
                }
            }
        }
        return blankCardCard;
    }
}
