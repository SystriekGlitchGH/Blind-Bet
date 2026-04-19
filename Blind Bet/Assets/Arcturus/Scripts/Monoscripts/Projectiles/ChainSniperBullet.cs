using UnityEngine;

public class ChainSniperBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, pm.playerStats.baseAbilityKnockback * pm.playerStats.GetAbilityKnockbackMod(), pm.playerStats.baseAbilityDamage*pm.playerStats.GetAbilityDamageMod()*0.5f);
                GameObject bomb = Instantiate(pm.prefabLib.miniRoyalBomb,enemy.transform.position,Quaternion.Euler(Vector3.zero));
                if(bomb.TryGetComponent(out MiniRoyalBomb mb))
                {
                    mb.pm = pm;
                    mb.followTransform = enemy.transform;
                }
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
