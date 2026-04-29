using System;
using NUnit.Framework.Internal;
using UnityEngine;
[Serializable]
public class Weapon
{
	public float baseAttack; // base attack of weapon
    public float baseAttackSpeed; // base attack speed of weapon
    public float baseKnockback; // base knockback of weapon
    public Vector2 baseAttackSize; // size of melee weapon hitbox
    public Vector2 baseParrySize; // size of parry hitbox

    
	public Weapon(Card.Suit s)
    {
        if(s == Card.Suit.diamond)
        {
            baseAttack = 30;
            baseAttackSpeed = 200;
            baseKnockback = 10;
            baseAttackSize = new Vector2(2.5f,3);
        }
        else if(s == Card.Suit.club)
        {
            baseAttack = 30;
            baseAttackSpeed = 10;
            baseKnockback = 14;
            baseAttackSize = new Vector2(4.5f, 3);
        }
        else if(s == Card.Suit.spade)
        {
            baseAttack = 25;
            baseAttackSpeed = 100;
            baseKnockback = 8;
            baseAttackSize = new Vector2(2f, 4);
        }
        baseParrySize = new Vector2(2,1f);
    }
}
