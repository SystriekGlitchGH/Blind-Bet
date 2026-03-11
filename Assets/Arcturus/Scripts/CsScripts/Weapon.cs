using UnityEngine;

public class Weapon
{
	public string type; // type of weapon: blade, gun, beam, summon
	public string name; // name of the weapon: sword, shotgun, dual beam
	public float baseAttack; // base attack of weapon
    public float baseAttackSpeed; // base attack speed of weapon
    public float baseKnockback; // base knockback of weapon
    public Vector2 baseAttackSize; // size of melee weapon hitbox
    public float baseAttackDistance; // distance away from player at center hitbox of melee weapon


	public Weapon(string n, string t)
    {
		name = n;
        if(n == "sword")
        {
            baseAttack = 6;
            baseAttackSpeed = 6;
            baseKnockback = 10;
            baseAttackSize = new Vector2(2.5f,3);
            baseAttackDistance = 2.5f;
        }
        else if(n == "axe")
        {
            baseAttack = 8;
            baseAttackSpeed = 2;
            baseKnockback = 20;
            baseAttackSize = new Vector2(3.5f, 2);
            baseAttackDistance = 2f;
        }
        else if(n == "spear")
        {
            baseAttack = 5;
            baseAttackSpeed = 4f;
            baseKnockback = 8;
            baseAttackSize = new Vector2(2f, 4);
            baseAttackDistance = 3f;
        }
    }
}
