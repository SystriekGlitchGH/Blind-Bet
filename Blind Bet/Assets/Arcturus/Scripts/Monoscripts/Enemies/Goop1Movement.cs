using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public class Goop1Movement : EnemyMovement
{
    public float dashForce;
    protected bool hasDashed;
    protected void Update()
    {
        if (isReadyingAttack && spriteRend.color != new Color32(210,225,0,255) )
            spriteRend.color = new Color32(210,225,0,255);
        if(!isReadyingAttack)
            spriteRend.color = new Color32(40,225,0,255);
        if (isAttacking && !hasDashed)
        {
            Debug.Log("Enemy attacked");
            rb2d.AddForce(PlayerDirection(target.transform.position)*dashForce, ForceMode2D.Impulse);
            hasDashed = true;
        }
    }
    protected override IEnumerator Attack()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        isAttacking = true; // is now attacking
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        hasDashed = false;
        yield return new WaitForSeconds(attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
}
