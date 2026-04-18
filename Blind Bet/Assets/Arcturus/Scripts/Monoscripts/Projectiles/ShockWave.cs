using UnityEngine;

public class ShockWave : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, 0, pm.playerStats.baseAbilityDamage*pm.playerStats.GetAbilityDamageMod()*1.5f);
                enemy.enemyStats.AddEffect("stun",3 * pm.playerStats.GetAbilityEffectDurationMod());
                enemiesHit++;
                if(enemiesHit == 1)
                {
                    Destroy(gameObject);
                }
            }
        }
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
