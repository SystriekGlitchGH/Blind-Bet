using UnityEngine;

public class Player
{
    public enum Suit
    {
        blank,diamond,club,spade
    }
    public Suit suit;
    public Weapon weapon;
    public float baseSpeed;
    public float AttackSpeed;

    public Player()
    {
        suit = Suit.blank;
        weapon = new Weapon(suit);
        baseSpeed = 10;
        AttackSpeed = 100;
    }
}
