using UnityEngine;

public class SpecSplinterBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "enemy")
        {
            if (collision.CompareTag("Player"))
            {
                PlayerMovement player = collision.GetComponent<PlayerMovement>();
                player.GetHit(em, em.enemyStats.baseKnockback);
                enemiesHit++;
                if(enemiesHit == 1)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if(bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, pm.playerStats.baseAbilityKnockback * pm.playerStats.GetAbilityKnockbackMod(), pm.playerStats.baseAbilityDamage*pm.playerStats.GetAbilityDamageMod());
                enemiesHit++;
                if(enemiesHit == 2)
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
