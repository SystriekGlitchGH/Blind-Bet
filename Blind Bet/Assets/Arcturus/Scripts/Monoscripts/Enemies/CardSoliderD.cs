using System.Collections;
using UnityEngine;

public class CardSoliderD : EnemyMovement
{
    public GameObject attackVisual;
    [SerializeField] Transform anchorTransform;
    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemy = new Enemy(10,30,6,2,3);
    }
    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(210,225,0,255);
        yield return new WaitForSeconds(0.3f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = new Color32(0,160,225,255);
        isAttacking = true; // is now attacking
        Debug.Log("Enemy attacked");
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(enemy.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    protected override IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150, 0, 0, 255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(225, 0, 150, 255);
        hasKnockback = false;
    }
}
