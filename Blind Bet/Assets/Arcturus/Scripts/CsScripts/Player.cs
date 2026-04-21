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
        public bool suited;
        public HandType type;

        public Hand(Card[] cards, Card.Suit suit, bool suited,HandType type)
        {
            this.cards = cards;
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
    public Ability passiveAbility2;

    public readonly Card blankCard = new Card(0,Card.Suit.blank);

    public EffectManager effectManager = new EffectManager();
    public bool hasStun, hasBurn, hasPoison, hasSlow, hasChill, hasFrozen, hasRecall, hasCharm, hasEnrage;
    
    public Weapon weapon;
    public float baseSpeed;
    public float maxHealth, currentHealth;
    public float maxChips, currentChips, chipLowThreshold;
    public float baseDashDistance, baseDashCooldown, baseDashDamage;
    public float baseParryTime, baseParryCooldown;
    public float baseAbilityDamage, baseAbilityKnockback, baseAbilityCooldown;

    public int kills;
    public Player()
    {
        activeSuit = Card.Suit.blank;
        weapon = new Weapon(activeSuit);
        baseSpeed = 10;
        maxHealth = 100;
        currentHealth = maxHealth;
        maxChips = 5000;
        currentChips = maxChips * 0.2f;
        chipLowThreshold = maxChips / 10;
        baseDashDistance = 20;
        baseDashCooldown = 0.5f;
        baseDashDamage = 5;
        baseParryTime = 0.2f;
        baseParryCooldown = 1;
        baseAbilityDamage = 10;
        baseAbilityKnockback = 10;
        activeHand = new Hand(new Card[5],Card.Suit.blank,false,HandType.none);
        passiveHand1 = new Hand(new Card[5], Card.Suit.blank, false, HandType.none);
        passiveHand2 = new Hand(new Card[5], Card.Suit.blank, false, HandType.none);
        FillHandBlank(activeHand);
        FillHandBlank(passiveHand1);
        FillHandBlank(passiveHand2);
    }
    public void CheckEffects()
    {
        if (effectManager.effects.FindIndex(x => x.name == "stun") != -1)
            hasStun = true;
        else hasStun = false;
        if (effectManager.effects.FindIndex(x => x.name == "burn") != -1)
            hasBurn = true;
        else hasBurn = false;
        if (effectManager.effects.FindIndex(x => x.name == "poison") != -1)
            hasPoison = true;
        else hasPoison = false;
        if (effectManager.effects.FindIndex(x => x.name == "slow") != -1)
            hasSlow = true;
        else hasSlow = false;
        if (effectManager.effects.FindIndex(x => x.name == "chill") != -1)
            hasChill = true;
        else hasChill = false;
        if (effectManager.effects.FindIndex(x => x.name == "frozen") != -1)
            hasFrozen = true;
        else hasFrozen = false;
        if (effectManager.effects.FindIndex(x => x.name == "recall") != -1)
            hasRecall = true;
        else hasRecall = false;
        if (effectManager.effects.FindIndex(x => x.name == "charm") != -1)
            hasCharm = true;
        else hasCharm = false;
        if(effectManager.effects.FindIndex(x => x.name == "enrage") != -1)
            hasEnrage = true;
        else hasEnrage = false;
    }
    public void AddEffect(string name, float time)
    {
        effectManager.effects.Add(new Effect(name, time));
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
    // unfinished
    public void SortHandCards(Hand hand, int handNum)
    {
        Hand sortedHand = new Hand(new Card[5], Card.Suit.blank,false, HandType.none);
        Card temp;
        bool swapped;
        for(int i = 0; i < hand.cards.Length; i++)
        {
            swapped = false;
            for(int j = 0; j < hand.cards.Length - i - 1; j++)
            {
                if(hand.cards[j].rank > hand.cards[j + 1].rank)
                {
                    temp = hand.cards[j];
                    hand.cards[j] = hand.cards[j+1];
                    hand.cards[j+1] = temp;
                    swapped = true;
                }
            }
            if (swapped == false)
                break;
        }
        sortedHand = hand;
        switch (handNum)
        {
            case 1:
                activeHand = sortedHand;
                break;
            case 2:
                passiveHand1 = sortedHand;
                break;
            case 3:
                passiveHand2 = sortedHand;
                break;
        }
    }
    public void AddCard()
    {
        activeHand.cards[0] = new Card(4, Card.Suit.diamond);
        activeHand.cards[1] = new Card(6, Card.Suit.diamond);
        activeHand.cards[2] = new Card(7, Card.Suit.diamond);
        activeHand.cards[3] = new Card(8, Card.Suit.diamond);
        activeHand.cards[4] = new Card(9, Card.Suit.diamond);

        passiveHand1.cards[0] = new Card(11, Card.Suit.spade);
        passiveHand1.cards[1] = new Card(11, Card.Suit.spade);
        passiveHand1.cards[2] = new Card(13, Card.Suit.spade);
        passiveHand1.cards[3] = new Card(14, Card.Suit.spade);
        passiveHand1.cards[4] = new Card(15, Card.Suit.spade);

        passiveHand2.cards[0] = new Card(11, Card.Suit.diamond);
        passiveHand2.cards[1] = new Card(11, Card.Suit.diamond);
        passiveHand2.cards[2] = new Card(12, Card.Suit.diamond);
        passiveHand2.cards[3] = new Card(12, Card.Suit.diamond);
        passiveHand2.cards[4] = new Card(13, Card.Suit.diamond);
    }
    // Abilities
    public Ability SetActiveAbility(Hand hand)
    {
        if (hand.type == HandType.high)
            activeAbility = new Ability("", "a1");
        else if(hand.type == HandType.pair)
            activeAbility = new Ability("Sharp Edge", "a2");
        else if(hand.type == HandType.twopair)
            activeAbility = new Ability("Swift Strike", "a3");
        else if(hand.type == HandType.kind3)
            activeAbility = new Ability("Sized Attack", "a4");
        else if(hand.type == HandType.flush)
            activeAbility = new Ability("Whirl Winds", "a5");
        else if(hand.type == HandType.straight)
            activeAbility = new Ability("Continuous Blade", "a6");
        else if(hand.type == HandType.fullhouse)
            activeAbility = new Ability("Echo slash", "a7");
        else if(hand.type == HandType.kind4)
            activeAbility = new Ability("Bi-Strike Blade", "a8");
        else if(hand.type == HandType.kind5)
            activeAbility = new Ability("Tri-Strike Blade", "a9");
        else if(hand.type == HandType.royalflush)
            activeAbility = new Ability("Explosive Sender", "a10");
        return null;
    }
    public Ability SetPassiveAbility1(Hand hand)
    {
        // diamond
        if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("", "n1d");
        else if(hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Swift Footing", "n2d");
        else if(hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Chilling Burst", "b3d");
        else if(hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Flashbang", "b4d");
        else if(hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Toxic Jynx", "n5d");
        else if(hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Splinter Carbine", "b6d");
        else if(hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Piercing Duet", "b7d");
        else if(hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Shocking Wheel", "n8d");
        else if(hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Freezing Wheel", "n9d");
        else if(hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility1 = new Ability("Combustion Carbine", "b10d");
        // heart
        else if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("", "n1h");
        else if (hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Dash Jack", "n2h");
        else if (hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Hyper Dash", "b3h");
        else if (hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Healing Sigil", "b4h");
        else if (hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Parasite Blade", "n5h");
        else if (hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Withering Pistol", "b6h");
        else if (hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Accult Sacrifice", "b7h");
        else if (hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Shielding Ward", "n8h");
        else if (hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Flash Ward", "n9h");
        else if (hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility1 = new Ability("Draining Mortar", "b10h");
        // club
        else if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("", "n1c");
        else if (hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Slicing Body", "n2c");
        else if (hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Unyielding Charge", "b3c");
        else if (hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Earth Break", "b4c");
        else if (hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Halting Blade", "n5c");
        else if (hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Roaring Shotgun", "b6c");
        else if (hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Hit and Run", "b7c");
        else if (hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Tectonic Assault", "n8c");
        else if (hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Tectonic Charge", "n9c");
        else if (hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility1 = new Ability("Holy Shotgun", "b10c");
        // spade
        else if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("", "n1s");
        else if (hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Crossing Rush", "n2s");
        else if (hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Shade Steps", "b3s");
        else if (hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Stunning Shockwave", "b4s");
        else if (hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Rememberance", "n5s");
        else if (hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Piercing Rifle", "b6s");
        else if (hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Radio Prism", "b7s");
        else if (hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Reaping Bayonette", "n8s");
        else if (hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Reaping Steps", "n9s");
        else if (hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility1 = new Ability("Chain Rifle", "b10s");

        return null;
    }
    public Ability SetPassiveAbility2(Hand hand)
    {
        // diamond
        if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("", "n1d");
        else if(hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Swift Footing", "n2d");
        else if(hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Chilling Burst", "b3d");
        else if(hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Flashbang", "b4d");
        else if(hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Toxic Jynx", "n5d");
        else if(hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Splinter Carbine", "b6d");
        else if(hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Piercing Duet", "b7d");
        else if(hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Shocking Wheel", "n8d");
        else if(hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Freezing Wheel", "n9d");
        else if(hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.diamond)
            passiveAbility2 = new Ability("Combustion Carbine", "b10d");
        // heart
        else if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("", "n1h");
        else if (hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Dash Jack", "n2h");
        else if (hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Hyper Dash", "b3h");
        else if (hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Healing Sigil", "b4h");
        else if (hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Parasite Blade", "n5h");
        else if (hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Withering Pistol", "b6h");
        else if (hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Accult Sacrifice", "b7h");
        else if (hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Shielding Ward", "n8h");
        else if (hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Flash Ward", "n9h");
        else if (hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.heart)
            passiveAbility2 = new Ability("Draining Mortar", "b10h");
        // club
        else if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("", "n1c");
        else if (hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Slicing Body", "n2c");
        else if (hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Unyielding Charge", "b3c");
        else if (hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Earth Break", "b4c");
        else if (hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Halting Blade", "n5c");
        else if (hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Roaring Shotgun", "b6c");
        else if (hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Hit and Run", "b7c");
        else if (hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Tectonic Assault", "n8c");
        else if (hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Tectonic Charge", "n9c");
        else if (hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.club)
            passiveAbility2 = new Ability("Holy Shotgun", "b10c");
        // spade
        else if (hand.type == HandType.high && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("", "n1s");
        else if (hand.type == HandType.pair && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Crossing Rush", "n2s");
        else if (hand.type == HandType.twopair && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Shade Steps", "b3s");
        else if (hand.type == HandType.kind3 && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Stunning Shockwave", "b4s");
        else if (hand.type == HandType.flush && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Rememberance", "n5s");
        else if (hand.type == HandType.straight && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Piercing Rifle", "b6s");
        else if (hand.type == HandType.fullhouse && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Radio Prism", "b7s");
        else if (hand.type == HandType.kind4 && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Reaping Bayonette", "n8s");
        else if (hand.type == HandType.kind5 && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Reaping Steps", "n9s");
        else if (hand.type == HandType.royalflush && GetHandSuit(hand) == Card.Suit.spade)
            passiveAbility2 = new Ability("Chain Rifle", "b10s");

        return null;
    }
    
    //chip stuff
    public int GetAttackChipUse()
    {
        float currentMin = 5;
        if(activeAbility.code == "a2")
            currentMin *= 2;
        else if(activeAbility.code == "a3")
            currentMin *= 3;
        else if(activeAbility.code == "a4")
            currentMin *= 4;
        else if(activeAbility.code == "a5")
            currentMin *= 5;
        else if(activeAbility.code == "a6")
            currentMin *= 6;
        else if(activeAbility.code == "a7")
            currentMin *= 7;
        else if(activeAbility.code == "a8")
            currentMin *= 8;
        else if(activeAbility.code == "a9")
            currentMin *= 9;
        else if(activeAbility.code == "a10")
            currentMin *= 10;
        if(passiveHand1.type == HandType.flush)
            currentMin += 15;
        if(passiveHand2.type == HandType.flush)
            currentMin += 15;
        return (int)currentMin;
    }
    public int GetAbility1ChipUse()
    {
        float currentMin = 20;
        if(passiveHand1.type == HandType.twopair)
            currentMin *= 3;
        else if(passiveHand1.type == HandType.kind3)
            currentMin *= 4;
        else if(passiveHand1.type == HandType.flush)
            currentMin *= 0;
        else if(passiveHand1.type == HandType.straight)
            currentMin *= 5;
        else if(passiveHand1.type == HandType.fullhouse)
            currentMin *= 8;
        else if(passiveHand1.type == HandType.kind4)
            currentMin *= 0;
        else if(passiveHand1.type == HandType.kind5)
            currentMin *= 0;
        else if(passiveHand1.type == HandType.royalflush)
            currentMin *= 15;
        if(passiveAbility1.code == "b3s")
        {
            currentMin = 0;
        }
        return (int)currentMin;
    }
    public int GetAbility2ChipUse()
    {
        float currentMin = 20;
        if(passiveHand2.type == HandType.twopair)
            currentMin *= 3;
        else if(passiveHand2.type == HandType.kind3)
            currentMin *= 4;
        else if(passiveHand2.type == HandType.flush)
            currentMin *= 0;
        else if(passiveHand2.type == HandType.straight)
            currentMin *= 5;
        else if(passiveHand2.type == HandType.fullhouse)
            currentMin *= 8;
        else if(passiveHand2.type == HandType.kind4)
            currentMin *= 0;
        else if(passiveHand2.type == HandType.kind5)
            currentMin *= 0;
        else if(passiveHand2.type == HandType.royalflush)
            currentMin *= 15;
        if(passiveAbility2.code == "b3s")
        {
            currentMin = 0;
        }
        return (int)currentMin;
    }
    public int GetHoldAbilityChipUse()
    {
        float currentMin = 0;
        if(passiveHand1.type == HandType.kind4)
            currentMin += 200;
        else if(passiveHand1.type == HandType.kind5)
            currentMin += 240;
        if(passiveHand2.type == HandType.kind4)
            currentMin += 200;
        else if(passiveHand2.type == HandType.kind5)
            currentMin += 240;
        return (int)currentMin;
    }
    public int GetDashChipUse()
    {
        float currentMin = 2;
        if(passiveHand1.type == HandType.pair)
            currentMin += 3;
        if(passiveHand2.type == HandType.pair)
            currentMin += 3;
        if(passiveAbility1.code == "b3s")
            currentMin += 5;
        if(passiveAbility2.code == "b3s")
            currentMin += 5;
        return (int)currentMin;
    }
    public int GetParryChipUse()
    {
        float currentMin = 20;
        return (int)currentMin;
    }
    // help methods
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    public void Heal(float heal)
    {
        currentHealth += heal;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    public void UpdateMaxHealth(float amount)
    {
        maxHealth += amount;
    }
    // modifiers
    public float GetAttackDamageMod()
    {
        float mod = 1;
        if(activeAbility.code == "a2" || activeAbility.code == "a3")
            mod += 0.3f;
        if(hasEnrage)
            mod += 1;
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
    public float GetAttackKnockbackMod()
    {
        float mod = 1;
        return mod;
    }
    public float GetSpeedMod()
    {
        float mod = 1;
        if(passiveAbility1.code == "n2d" || passiveAbility2.code == "n2d")
            mod += 0.2f;
        if(currentChips <= chipLowThreshold)
            mod /= 2;
        return mod;
    }
    public float GetDamageMod()
    {
        float mod = 1;
        return mod;
    }
    public float GetAbilityDamageMod()
    {
        float mod = 1;
        return mod;
    }
    public float GetAbilityKnockbackMod()
    {
        float mod = 1;
        return mod;
    }
    public float GetAbilitySizeMod()
    {
        float mod = 1;
        return mod;
    }
    public float GetAbilityCooldownMod()
    {
        float mod = 1;
        return mod;
    }
    public float GetAbilityEffectDurationMod()
    {
        float mod = 1;
        return mod;
    }
    public float GetDashCooldownMod()
    {
        float mod = 1;
        if (passiveAbility1.code == "n2h" || passiveAbility2.code == "n2h")
            mod -= 0.25f;
        return mod;
    }
    public float GetDashDamageMod()
    {
        float mod = 1;
        if (passiveAbility1.code == "n2c" || passiveAbility2.code == "n2c")
            mod += 0.2f;
        return mod;
    }
    public float GetDashdistanceMod()
    {
        float mod = 1;
        if(passiveAbility1.code == "n2s" || passiveAbility2.code == "n2s")
            mod += 0.2f;
        return mod;
    }
}
