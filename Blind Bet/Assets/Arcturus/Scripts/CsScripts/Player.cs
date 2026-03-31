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

    public Ability activeAbility;
    public Ability passiveAbility1;
    public Ability passiveability2;

    public readonly Card blankCard = new Card(0,Card.Suit.blank);

    public Weapon weapon;
    public float baseSpeed;
    public float baseDashDistance, baseDashCooldown;
    public float baseParryTime, baseParryCooldown;

    public float attackModifier;
    public Player()
    {
        activeSuit = Card.Suit.blank;
        weapon = new Weapon(activeSuit);
        baseSpeed = 10;
        baseDashDistance = 20;
        baseDashCooldown = 0.5f;
        baseParryTime = 0.2f;
        baseParryCooldown = 1;
        activeHand = new Hand(new Card[5],Card.Suit.blank,false,HandType.none);
        passiveHand1 = new Hand(new Card[5], Card.Suit.blank, false, HandType.none);
        passiveHand2 = new Hand(new Card[5], Card.Suit.blank, false, HandType.none);
        FillHandBlank(activeHand);
        FillHandBlank(passiveHand1);
        FillHandBlank(passiveHand2);
    }
    
    // fills the selected hand with blank cards
    public void FillHandBlank(Hand hand)
    {
        for(int i = 0; i <hand.cards.Length; i++)
        {
            hand.cards[i] = blankCard;
        }
    }
    // checks if the hand is suited(all one suit)
    public bool IsSuited(Hand hand)
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
    // returns the card suit of the hand
    public Card.Suit GetHandSuit(Hand hand)
    {
        return hand.cards[0].suit;
    }

    public void SetHandType(Hand hand, int handNum)
    {
        if(hand.cards[0].rank == 10 && hand.cards[1].rank == 11 && hand.cards[2].rank == 12 && hand.cards[3].rank == 13 && hand.cards[4].rank == 14 && IsSuited(hand))
            hand.type = HandType.royalflush;
        else if (hand.cards[1].rank == hand.cards[0].rank && hand.cards[2].rank == hand.cards[0].rank && hand.cards[3].rank == hand.cards[0].rank && hand.cards[4].rank == hand.cards[0].rank)
            hand.type = HandType.kind5;
        else if (hand.cards[1].rank == hand.cards[0].rank && hand.cards[2].rank == hand.cards[0].rank && hand.cards[3].rank == hand.cards[0].rank)
            hand.type = HandType.kind4;
        else if(hand.cards[1].rank == hand.cards[0].rank && hand.cards[2].rank == hand.cards[0].rank && hand.cards[4].rank == hand.cards[3].rank)
            hand.type = HandType.fullhouse;
        else if(hand.cards[1].rank == hand.cards[0].rank+1 && hand.cards[2].rank == hand.cards[1].rank+1 && hand.cards[3].rank == hand.cards[2].rank+1 && hand.cards[4].rank == hand.cards[3].rank+1)
            hand.type = HandType.straight;
        else if(hand.cards[1].suit == hand.cards[0].suit && hand.cards[2].suit == hand.cards[0].suit && hand.cards[3].suit == hand.cards[0].suit && hand.cards[4].suit == hand.cards[0].suit)
            hand.type = HandType.flush;
        else if (hand.cards[1].rank == hand.cards[0].rank && hand.cards[2].rank == hand.cards[0].rank)
            hand.type = HandType.kind3;
        else if (hand.cards[1].rank == hand.cards[0].rank && hand.cards[3].rank == hand.cards[2].rank)
            hand.type = HandType.twopair;
        else if (hand.cards[1].rank == hand.cards[0].rank)
            hand.type = HandType.pair;
        else if(hand.cards[0].rank != hand.cards[1].rank)
            hand.type = HandType.high;
        else
            hand.type = HandType.none;
        switch (handNum)
        {
            case 1:
                activeHand.type = hand.type;
                break;
            case 2:
                passiveHand1.type = hand.type;
                break;
            case 3:
                passiveHand2.type = hand.type;
                break;
        }
    }
    public void AddCard()
    {
        activeHand.cards[0] = new Card(4, Card.Suit.spade);
        activeHand.cards[1] = new Card(4, Card.Suit.spade);
        activeHand.cards[2] = new Card(4, Card.Suit.spade);
        activeHand.cards[3] = new Card(4, Card.Suit.spade);
        activeHand.cards[4] = new Card(4, Card.Suit.spade);
    }

    // Abilities
    public Ability SetActiveAbility(Hand hand)
    {
        if (hand.type == HandType.high)
            activeAbility = new Ability(Ability.Type.active, "", "a1");
        else if(hand.type == HandType.pair)
            activeAbility = new Ability(Ability.Type.active, "Sharp Edge", "a2");
        else if(hand.type == HandType.twopair)
            activeAbility = new Ability(Ability.Type.active, "Swift Strike", "a3");
        else if(hand.type == HandType.kind3)
            activeAbility = new Ability(Ability.Type.active, "Sized Attack", "a4");
        else if(hand.type == HandType.flush)
            activeAbility = new Ability(Ability.Type.active, "Whirl Winds", "a5");
        else if(hand.type == HandType.straight)
            activeAbility = new Ability(Ability.Type.active, "Continuous Blade", "a6");
        else if(hand.type == HandType.fullhouse)
            activeAbility = new Ability(Ability.Type.active, "Echo slash", "a7");
        else if(hand.type == HandType.kind4)
            activeAbility = new Ability(Ability.Type.active, "Bi-Strike Blade", "a8");
        else if(hand.type == HandType.kind5)
            activeAbility = new Ability(Ability.Type.active, "Tri-Strike Blade", "a9");
        else if(hand.type == HandType.royalflush)
            activeAbility = new Ability(Ability.Type.active, "Explosive Sender", "a10");
        return null;
    }
    
    // modifiers
    public float GetAttackDamageMod()
    {
        float mod = 1;
        if(activeAbility.code == "a2" || activeAbility.code == "a3")
            mod += 0.3f;
        return mod;
    }
    public float GetAttackSpeedMod()
    {
        float mod = 1;
        if(activeAbility.code == "a3")
            mod += 0.2f;
        return mod;
    }
    public float GetAttackSizeMod()
    {
        float mod = 1;
        if(activeAbility.code == "a4")
            mod += 0.3f;
        return mod;
    }
}
