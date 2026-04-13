using UnityEngine;

public class SoundWave : Bullet
{
    protected override void Update()
    {
        base.Update();
        transform.localScale += new Vector3(2,0.5f,0) * 3 * Time.deltaTime;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, 0, pm.playerStats.baseAbilityDamage*pm.playerStats.GetAbilityDamageMod()*0.05f);
                enemy.enemyStats.AddEffect("stun",3 * pm.playerStats.GetAbilityEffectDurationMod());
                enemiesHit++;
                if(enemiesHit == 255)
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
