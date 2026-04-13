using UnityEngine;

public class ContBlade : Bullet
{
    protected void Start()
    {
        transform.localScale = pm.playerStats.weapon.baseAttackSize*pm.playerStats.GetAttackSizeMod();
    }
    protected override void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= 0.5)
        {
            Destroy(gameObject);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, pm.playerStats.weapon.baseKnockback, pm.playerStats.weapon.baseAttack*pm.playerStats.GetAttackDamageMod());
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
