using UnityEngine;

public class RoyalBomb : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "enemy")
        {
            if (collision.CompareTag("Player"))
            {
                PlayerMovement player = collision.GetComponent<PlayerMovement>();
                player.GetHit(em, em.enemy.baseKnockback);
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
                enemy.GetHit(pm, pm.playerStats.weapon.baseKnockback, pm.playerStats.weapon.baseAttack*pm.playerStats.GetAttackDamageMod());
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
    private void Explode()
    {
        
    }
}