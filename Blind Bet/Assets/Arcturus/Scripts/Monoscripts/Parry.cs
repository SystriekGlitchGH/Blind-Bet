using System.Collections;
using UnityEngine;

public class Parry : MonoBehaviour
{
    public float attackAngle;
    public Player playerStats;
    public PlayerMovement pm;
    public LayerMask boxLayer;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyMovement em = collision.GetComponent<EnemyMovement>();
            if (em.IsAttacking())
            {
                em.setVelocity(Vector2.zero);
                Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
                Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y/2*2+1);
                RaycastHit2D[] hits = Physics2D.BoxCastAll(pm.transform.position + (Vector3)position, playerStats.weapon.baseAttackSize*2, attackAngle, Vector2.zero,0,boxLayer); 
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
                    {
                        enemy.GetHit(pm, playerStats.weapon.baseKnockback*2);
                    }
                }
                // makes an attack visual sprite when using a melee attack
                StartCoroutine(RetaliationTimer());
            }
        }
    }

    private IEnumerator RetaliationTimer()
    {
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y/2*2+1);
        GameObject attack = Instantiate(pm.attackVisual, pm.transform.position + (Vector3)position, pm.anchorTransform.rotation, pm.anchorTransform);
        attack.transform.localScale = playerStats.weapon.baseAttackSize*2;
        yield return new WaitForSeconds(0.1f);
        Destroy(attack);
    }
}

