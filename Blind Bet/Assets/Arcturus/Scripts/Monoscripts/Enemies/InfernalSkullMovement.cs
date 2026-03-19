using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class InfernalSkullMovement : EnemyMovement
{
    public GameObject attackVisual;
    public float attackRadius;
    public LayerMask circleLayer;
    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(210, 225, 0, 255);
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = new Color32(230, 80, 180, 255);
        isAttacking = true; // is now attacking

        Debug.Log("Enemy attacked");
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, attackRadius, Vector2.zero, 0, circleLayer);
        if(hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
        {
            player.GetHit(this, baseKnockback);
        }
        GameObject attack = Instantiate(attackVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale *= attackRadius*2;
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        Destroy(attack);
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    protected override IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150, 0, 0, 255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(230, 80, 180, 255);
        hasKnockback = false;
    }
    private RaycastHit2D MakeBoxCast()
    {
        return Physics2D.CircleCast(transform.position, attackRadius, Vector2.zero, 0, circleLayer);
    }
}
