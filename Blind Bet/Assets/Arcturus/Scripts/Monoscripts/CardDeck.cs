using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeck", menuName = "Scriptable Objects/CardDeck")]
public class CardDeck : ScriptableObject
{
    [Header("Card Arrays")]
    public List<GameObject> diamonds;
    public List<GameObject> hearts;
    public List<GameObject> clubs;
    public List<GameObject> spades;
    public GameObject blankCard;

    public GameObject GetUIObjectFromCard(Card card)
    {
        if(card.suit == Card.Suit.diamond)
        {
            for(int i = 0; i < diamonds.Count; i++)
            {
                if(diamonds[i].TryGetComponent(out Card attachedCard))
                {
                    if(attachedCard == card)
                        return diamonds[i];
                }
            }
        }
        else if(card.suit == Card.Suit.heart)
        {
            for(int i = 0; i < hearts.Count; i++)
            {
                if(hearts[i].TryGetComponent(out Card attachedCard))
                {
                    if(attachedCard == card)
                        return hearts[i];
                }
            }
        }
        else if(card.suit == Card.Suit.club)
        {
            for(int i = 0; i < clubs.Count; i++)
            {
                if(clubs[i].TryGetComponent(out Card attachedCard))
                {
                    if(attachedCard == card)
                        return clubs[i];
                }
            }
        }
        else if(card.suit == Card.Suit.spade)
        {
            for(int i = 0; i < spades.Count; i++)
            {
                if(spades[i].TryGetComponent(out Card attachedCard))
                {
                    if(attachedCard == card)
                        return spades[i];
                }
            }
        }
        return blankCard;
    }
}
