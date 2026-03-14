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
    public float dashDistance, dashCooldown;
    public float baseParryTime, parryCooldown;
    public Player()
    {
        suit = Suit.blank;
        weapon = new Weapon(suit);
        baseSpeed = 10;
        AttackSpeed = 100;
        dashDistance = 20;
        dashCooldown = 0.5f;
        baseParryTime = 0.2f;
        parryCooldown = 1;
    }
}
