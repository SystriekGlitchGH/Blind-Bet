using UnityEngine;

public class Enemy
{
    public float baseDamage;
    public float maxHealth;
    public float baseKnockback;
    public float topSpeed;

    public float attackCooldown;

    public float currentHealth;

    public Enemy(float baseDamage, float maxHealth, float baseKnockback, float topSpeed, float attackCooldown)
    {
        this.baseDamage = baseDamage;
        this.maxHealth = maxHealth;
        this.baseKnockback = baseKnockback;
        this.topSpeed = topSpeed;
        this.attackCooldown = attackCooldown;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
