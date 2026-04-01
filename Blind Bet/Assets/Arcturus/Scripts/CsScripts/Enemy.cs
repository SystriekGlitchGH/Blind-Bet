using UnityEngine;

public class Enemy
{
    public EffectManager effectManager;
    public bool hasStun, hasBurn, hasPoison, hasSlow, hasChill, hasFrozen, hasRecall, hasCharm;
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
    public void CheckEffects()
    {
        if(effectManager.effects.FindIndex(x => x.name == "stun") != -1)
            hasStun = true;
        if(effectManager.effects.FindIndex(x => x.name == "burn") != -1)
            hasBurn = true;
        if(effectManager.effects.FindIndex(x => x.name == "poison") != -1)
            hasPoison = true;
        if(effectManager.effects.FindIndex(x => x.name == "Slow") != -1)
            hasSlow = true;
        if(effectManager.effects.FindIndex(x => x.name == "Chill") != -1)
            hasChill = true;
        if(effectManager.effects.FindIndex(x => x.name == "Frozen") != -1)
            hasFrozen = true;
        if(effectManager.effects.FindIndex(x => x.name == "Recall") != -1)
            hasRecall = true;
        if(effectManager.effects.FindIndex(x => x.name == "Charm") != -1)
            hasBurn = true;
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

    public float GetSpeedMod()
    {
        float mod = 1;
        if (hasChill)
            mod -= 0.3f;
        return mod;
    }
    public float GetDamageMod()
    {
        float mod = 1;
        if (hasChill)
            mod += 0.2f;
        return mod;
    }
}
