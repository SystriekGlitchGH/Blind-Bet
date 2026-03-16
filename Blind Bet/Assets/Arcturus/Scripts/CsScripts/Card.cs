using UnityEngine;

public class Card
{
    public enum Suit
    {
        blank,diamond,heart,club,spade
    }
    public int rank;
    public Suit suit;

    public Card(int rank, Suit suit)
    {
        this.rank = rank;
        this.suit = suit;
    }
}
