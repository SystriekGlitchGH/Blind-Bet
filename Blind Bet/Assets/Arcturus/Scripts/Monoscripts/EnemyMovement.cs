using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    public PlayerMovement target;
    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb2d;
    [SerializeField] protected SpriteRenderer spriteRend;
    [SerializeField] protected Enemy enemy;

    [Header("Movement Stats")]
    public float acceleration;
    public float topSpeed;
    public float friction;
    // variables to help with movement
    protected float distance;
    protected bool hasKnockback;
    public float knockbackTime;

    [Header("Attack stats")]
    public float attackCooldown;
    protected bool canAttack = true, isReadyingAttack, isAttacking;
    public float AttackRange;

    //other
    protected float colliderPushForce = 8;

    protected void Start()
    {
        rb2d.linearDamping = friction;
    }
    protected void FixedUpdate()
    {
        if(target != null)
        {
            if (hasKnockback || isAttacking)
            {
                return;
            }
            distance = PlayerDistance(target.transform.position);
            if(distance > 2)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = PlayerDirection(target.transform.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), topSpeed);
                rb2d.linearVelocity = velocity;
            }
            if(distance < AttackRange && canAttack)
            {
                ActivateAttack();
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
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.rb2d.AddForce(PlayerDirection(pm.transform.position)*colliderPushForce);
        }
        else if (collision.CompareTag("Enemy"))
        {
            EnemyMovement em = collision.GetComponent<EnemyMovement>();
            em.rb2d.AddForce(PlayerDirection(em.transform.position)*colliderPushForce);
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    public void Hit(PlayerMovement attacker, float knockback)
    {
        StartCoroutine(GetHit());
        rb2d.AddForce(attacker.DirectionToVector()*knockback,ForceMode2D.Impulse);
    }
    public void ActivateAttack()
    {
        StartCoroutine(Attack());
    }

    protected IEnumerator GetHit()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150,0,0,255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(90,215,0,255);
        hasKnockback = false;
    }
    protected virtual IEnumerator Attack()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        yield return new WaitForSeconds(0.3f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        isAttacking = true; // is now attacking
        yield return new WaitForSeconds(0.5f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    //movement help methods
    protected float PlayerDistance(Vector2 playerPos)
    {
        return Vector2.Distance((Vector2)transform.position, playerPos);
    }
    protected Vector2 PlayerDirection(Vector2 playerPos)
    {
        return (playerPos - (Vector2)transform.position).normalized;
    }
}
