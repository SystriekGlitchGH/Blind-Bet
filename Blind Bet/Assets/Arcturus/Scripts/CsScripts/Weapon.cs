using NUnit.Framework.Internal;
using UnityEngine;

public class Weapon
{
	public float baseAttack; // base attack of weapon
    public float baseAttackSpeed; // base attack speed of weapon
    public float baseKnockback; // base knockback of weapon
    public Vector2 baseAttackSize; // size of melee weapon hitbox
    public Vector2 baseParrySize; // size of parry hitbox


	public Weapon(Player.Suit s)
    {
        if(s == Player.Suit.diamond)
        {
            baseAttack = 6;
            baseAttackSpeed = 200;
            baseKnockback = 10;
            baseAttackSize = new Vector2(2.5f,3);
        }
        else if(s == Player.Suit.club)
        {
            baseAttack = 8;
            baseAttackSpeed = 0;
            baseKnockback = 20;
            baseAttackSize = new Vector2(3.5f, 2);
        }
        else if(s == Player.Suit.spade)
        {
            baseAttack = 5;
            baseAttackSpeed = 100;
            baseKnockback = 8;
            baseAttackSize = new Vector2(2f, 4);
        }
        baseParrySize = new Vector2(2,0.5f);
    }
}
