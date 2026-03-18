using System;
using UnityEngine;

public class Player
{
    public enum HandType
    {
        none,high,pair,twopair,kind3,flush,straight,fullhouse,kind4,kind5,royalflush
    }
    public struct Hand
    {
        public Card[] cards;
        public Card.Suit suit;
        public bool suited;
        public HandType type;

        public Hand(Card[] cards, Card.Suit suit, bool suited,HandType type)
        {
            this.cards = cards;
            this.suit = suit;
            this.suited = suited;
            this.type = type;
        }
    }
    public Card.Suit activeSuit;

    public Hand activeHand; 
    public Hand passiveHand1;
    public Hand passiveHand2;

    public readonly Card blankCard = new Card(0,Card.Suit.blank);

    public Weapon weapon;
    public float baseSpeed;
    public float AttackSpeed;
    public float dashDistance, dashCooldown;
    public float baseParryTime, parryCooldown;
    public Player()
    {
        activeSuit = Card.Suit.blank;
        weapon = new Weapon(activeSuit);
        baseSpeed = 10;
        AttackSpeed = 100;
        dashDistance = 20;
        dashCooldown = 0.5f;
        baseParryTime = 0.2f;
        parryCooldown = 1;
        activeHand = new Hand(new Card[5],Card.Suit.blank,false,HandType.none);
        passiveHand1 = new Hand(new Card[5], Card.Suit.blank, false, HandType.none);
        passiveHand2 = new Hand(new Card[5], Card.Suit.blank, false, HandType.none);
        FillHandBlank(activeHand);
        FillHandBlank(passiveHand1);
        FillHandBlank(passiveHand2);
    }

    public void FillHandBlank(Hand hand)
    {
        for(int i = 0; i <hand.cards.Length; i++)
        {
            hand.cards[i] = blankCard;
        }
    }

    public bool IsSuited(Hand hand)
    {
        if(hand.type != HandType.none)
        {
            Card.Suit initialSuit = hand.cards[0].suit;
            foreach(Card c in hand.cards)
            {
                if(c != blankCard)
                {
                    if(initialSuit != c.suit)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public Card.Suit GetHandSuit(Hand hand)
    {
        if (IsSuited(hand))
        {
            return hand.cards[0].suit;
        }
        else
        {
            return Card.Suit.blank;
        }
    }
    public void AddCard()
    {
        Card card = new Card(2, Card.Suit.diamond);
        activeHand.cards[0] = card;
        activeHand.cards[1] = card;
    }
}
