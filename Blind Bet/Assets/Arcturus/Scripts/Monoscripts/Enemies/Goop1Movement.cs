using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public class Goop1Movement : EnemyMovement
{
    public float dashForce;
    protected override void FixedUpdate()
    {
        if (target != null)
        {
            if (hasKnockback || isAttacking)
            {
                return;
            }
            if(distance < AttackRange && canAttack)
            {
                StartCoroutine(AttackTimer());
            }
            if(distance < 2 && !isAttacking)
            {
                rb2d.linearDamping = friction;
            }
            if(distance > 40)
            {
                target = null;
                rb2d.linearDamping = friction;
            }
        }
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttacking)
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.GetHit(this,baseKnockback);
        }
    }

    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(210,225,0,255);
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = new Color32(40,225,0,255);
        isAttacking = true; // is now attacking
        Debug.Log("Enemy attacked");
        rb2d.AddForce(PlayerDirection(target.transform.position)*dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
}
