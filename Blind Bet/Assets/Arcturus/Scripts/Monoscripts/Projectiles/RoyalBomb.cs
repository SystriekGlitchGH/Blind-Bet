using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RoyalBomb : Bullet
{
    [SerializeField] GameObject explosionVisual;
    [SerializeField] SpriteRenderer spriteRend;
    protected override void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > 3)
        {
            StartCoroutine(ExplodeTimer());
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
                if(enemiesHit == 1)
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
    private IEnumerator ExplodeTimer()
    {
        rb2d.linearVelocity = Vector2.zero;
        spriteRend.enabled = false;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2.5f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.collider.TryGetComponent(out EnemyMovement enemy))
                enemy.GetHit(this, pm.playerStats.weapon.baseKnockback, pm.playerStats.weapon.baseAttack * pm.playerStats.GetAttackDamageMod() * 3f);
        }
        GameObject attack = Instantiate(explosionVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(5f, 5f);
        yield return new WaitForSeconds(0.2f);
        Destroy(attack);
        Destroy(gameObject);
    }
}