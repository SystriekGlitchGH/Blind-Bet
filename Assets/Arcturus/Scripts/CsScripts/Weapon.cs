using UnityEngine;

public class Weapon
{
	public float baseAttack; // base attack of weapon
    public float baseAttackSpeed; // base attack speed of weapon
    public float baseKnockback; // base knockback of weapon
    public Vector2 baseAttackSize; // size of melee weapon hitbox
    public float baseAttackDistance; // distance away from player at center hitbox of melee weapon


	public Weapon(Player.Suit s)
    {
        if(s == Player.Suit.diamond)
        {
            baseAttack = 6;
            baseAttackSpeed = 6;
            baseKnockback = 10;
            baseAttackSize = new Vector2(2.5f,3);
            baseAttackDistance = 2.5f;
        }
        else if(s == Player.Suit.club)
        {
            baseAttack = 8;
            baseAttackSpeed = 2;
            baseKnockback = 20;
            baseAttackSize = new Vector2(3.5f, 2);
            baseAttackDistance = 2f;
        }
        else if(s == Player.Suit.spade)
        {
            baseAttack = 5;
            baseAttackSpeed = 4f;
            baseKnockback = 8;
            baseAttackSize = new Vector2(2f, 4);
            baseAttackDistance = 3f;
        }
    }
}
