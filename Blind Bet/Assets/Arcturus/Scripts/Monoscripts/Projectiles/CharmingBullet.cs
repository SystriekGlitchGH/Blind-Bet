using UnityEngine;

public class CharmingBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, 0, pm.playerStats.baseAbilityDamage * pm.playerStats.GetAbilityDamageMod() * 2f);
                enemy.enemyStats.TakeMaxDamage(pm.playerStats.baseAbilityDamage * pm.playerStats.GetAbilityDamageMod());
                enemiesHit++;
                if (enemiesHit == 1)
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
