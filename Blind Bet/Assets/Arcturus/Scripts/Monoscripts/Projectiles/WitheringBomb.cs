using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class WitheringBomb : Bullet
{
    [SerializeField] GameObject explosionVisual;
    [SerializeField] SpriteRenderer spriteRend;
    private bool exploded;
    protected override void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 3 && !exploded)
        {
            StartCoroutine(ExplodeTimer());
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (bulletType == "player")
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
                enemy.GetHit(pm, pm.playerStats.baseAbilityKnockback * 0.5f, pm.playerStats.baseAbilityDamage * pm.playerStats.GetAbilityDamageMod());
                pm.playerStats.Heal(pm.playerStats.baseAbilityDamage * pm.playerStats.GetAbilityDamageMod()*0.1f);
                enemiesHit++;
                if (enemiesHit == 1)
                {
                    StartCoroutine(ExplodeTimer());
                }
            }
        }
        if (collision.CompareTag("Wall"))
        {
            StartCoroutine(ExplodeTimer());
        }
    }
    private IEnumerator  ExplodeTimer()
    {
        exploded = true;
        rb2d.linearVelocity = Vector2.zero;
        spriteRend.enabled = false;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2.5f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.collider.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHit(this, pm.playerStats.baseAbilityKnockback, pm.playerStats.baseAbilityDamage * pm.playerStats.GetAbilityDamageMod() * 4);
                enemy.enemyStats.TakeMaxDamage(pm.playerStats.baseAbilityDamage * pm.playerStats.GetAbilityDamageMod()*2);
                pm.playerStats.Heal(pm.playerStats.baseAbilityDamage * pm.playerStats.GetAbilityDamageMod() * 4 * 0.1f);
            }
        }
        GameObject attack = Instantiate(explosionVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(5f, 5f);
        yield return new WaitForSeconds(0.2f);
        Destroy(attack);
        Destroy(gameObject);
    }
}
