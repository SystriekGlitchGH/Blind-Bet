using UnityEngine;

public class Ability
{
    public enum Type
    {
        active, passiveNatural, passiveButton
    }
    public Type type;
    public string name;
    public string code;
    public Card.Suit suit;
    public Ability(Type type, string name, string code)
    {
        this.type = type;
        this.name = name;
        this.code = code;
    }
    public Ability(Type type, string name, string code, Card.Suit suit)
    {
        this.type = type;
        this.name = name;
        this.code = code;
        this.suit = suit;
    }
}
