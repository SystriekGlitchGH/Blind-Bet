using UnityEngine;
using UnityEngine.Rendering;

public class Enemy
{
    public EffectManager effectManager = new EffectManager();
    public bool hasStun, hasBurn, hasPoison, hasSlow, hasChill, hasFrozen, hasRecall, hasCharm, hasEnrage;
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
        if (effectManager.effects.FindIndex(x => x.name == "stun") != -1)
            hasStun = true;
        else hasStun = false;
        if (effectManager.effects.FindIndex(x => x.name == "burn") != -1)
            hasBurn = true;
        else hasBurn = false;
        if (effectManager.effects.FindIndex(x => x.name == "poison") != -1)
            hasPoison = true;
        else hasPoison = false;
        if (effectManager.effects.FindIndex(x => x.name == "slow") != -1)
            hasSlow = true;
        else hasSlow = false;
        if(effectManager.effects.FindIndex(x => x.name == "chill") != -1)
            hasChill = true;
        else hasChill = false;
        if(effectManager.effects.FindIndex(x => x.name == "frozen") != -1)
            hasFrozen = true;
        else hasFrozen = false;
        if(effectManager.effects.FindIndex(x => x.name == "recall") != -1)
            hasRecall = true;
        else hasRecall = false;
        if(effectManager.effects.FindIndex(x => x.name == "charm") != -1)
            hasCharm = true;
        else hasCharm = false;
        if(effectManager.effects.FindIndex(x => x.name == "enrage") != -1)
            hasEnrage = true;
        else hasEnrage = false;
    }
    public void AddEffect(string name, float time)
    {
        effectManager.effects.Add(new Effect(name,time));
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
    public void TakeMaxDamage(float damage)
    {
        maxHealth -= damage;
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
            mod -= 0.5f;
        if(hasSlow)
            mod -= 0.3f;
        if(hasEnrage)
            mod += 1;
        if(mod <= 0)
            mod = 0.05f;
        return mod;
    }
    public float GetDamageMod()
    {
        float mod = 1;
        if (hasChill)
            mod += 0.2f;
        if(hasFrozen)
            mod += 2;
        return mod;
    }
    public float GetAttackDamageMod()
    {
        float mod = 1;
        if (hasPoison)
            mod -= 0.3f;
        return mod;
    }
    public float GetAttackSpeedMod()
    {
        float mod = 1;
        return mod;
    }
}
