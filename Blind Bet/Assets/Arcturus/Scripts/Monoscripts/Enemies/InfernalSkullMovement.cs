using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class InfernalSkullMovement : EnemyMovement
{
    public GameObject attackVisual;
    public float attackRadius;
    public LayerMask circleLayer;

    //Pathfinding
    public Node currentNode;
    public List<Node> path;

    public enum StateMachine
    {
        patrol,engage,evade
    }
    public StateMachine currentState;

    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemy = new Enemy(10,20,5,2,2);
        currentState = StateMachine.patrol;
    }
    private void Update()
    {
        switch (currentState)
        {
            case StateMachine.patrol:
                Patrol();
                break;
            case StateMachine.engage:
                Engage();
                break;
            case StateMachine.evade:
                Evade();
                break;
        }
        if(enemyTarget == null && currentState != StateMachine.patrol)
        {
            currentState = StateMachine.patrol;
            path.Clear();
        }
        else if(enemyTarget != null && currentState != StateMachine.engage)
        {
            currentState = StateMachine.engage;
            path.Clear();
        }
        else if(enemyTarget != null && currentState != StateMachine.evade)
        {
            currentState = StateMachine.evade;
            path.Clear();
        }
    }
    protected override void FixedUpdate()
    {
        if(enemyTarget != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position,TargetDirection(enemyTarget.transform.position),3,hitLayer);
            Debug.DrawRay(rb2d.position, TargetDirection(enemyTarget.transform.position) * 3f, Color.red);
            if (hasKnockback || isAttacking)
            {
                return;
            }
            distance = TargetDistance(enemyTarget.transform.position);
            if(distance > stopRange)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemy.topSpeed);
                rb2d.linearVelocity = velocity;
            }
            if(distance < AttackRange && canAttack)
            {
                StartCoroutine(AttackTimer());
            }
            if(distance < stopRange && !isAttacking)
            {
                rb2d.linearDamping = friction;
            }
            if(distance > 40)
            {
                enemyTarget = null;
                rb2d.linearDamping = friction;
            }
        }
    }
    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(230, 80, 180, 255);
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = new Color32(200, 200, 200, 255);
        isAttacking = true; // is now attacking

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, attackRadius, Vector2.zero, 0, circleLayer);
        if(hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
        {
            player.GetHit(this, enemy.baseKnockback);
        }
        GameObject attack = Instantiate(attackVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale *= attackRadius*2;
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        Destroy(attack);
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(enemy.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    protected override IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150, 0, 0, 255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(200, 200, 200, 255);
        hasKnockback = false;
    }

    private void CreatePath()
    {
        
    }
    private void Patrol()
    {
        
    }
    private void Engage()
    {
        
    }
    private void Evade()
    {
        
    }
}
