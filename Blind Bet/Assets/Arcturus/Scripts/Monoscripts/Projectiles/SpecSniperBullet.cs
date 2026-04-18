using UnityEngine;

public class SpecSniperBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, pm.playerStats.baseAbilityKnockback * pm.playerStats.GetAbilityKnockbackMod(), pm.playerStats.baseAbilityDamage*pm.playerStats.GetAbilityDamageMod()*1.5f);
                enemiesHit++;
                if(enemiesHit == 8)
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
