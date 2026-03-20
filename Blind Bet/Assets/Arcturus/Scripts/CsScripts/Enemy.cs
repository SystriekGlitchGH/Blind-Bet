using UnityEngine;

public class Enemy
{
    public float baseDamage;
    public float baseHealth;
    public float baseKnockback;
    public float topSpeed;

    public float attackCooldown;

    public Enemy(float baseDamage, float baseHealth, float baseKnockback, float topSpeed, float attackCooldown)
    {
        this.baseDamage = baseDamage;
        this.baseHealth = baseHealth;
        this.baseKnockback = baseKnockback;
        this.topSpeed = topSpeed;
        this.attackCooldown = attackCooldown;
    }

    public void TakeDamage(float damage)
    {
        baseHealth -= damage;
    }
}
