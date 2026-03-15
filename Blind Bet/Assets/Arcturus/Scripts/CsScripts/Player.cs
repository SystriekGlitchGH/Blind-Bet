using UnityEngine;

public class Player
{
    public Card.Suit activeSuit;
    public Card[] activeHand; 
    public Card[] passiveHand1;
    public Card[] passiveHand2;
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
    }
}
