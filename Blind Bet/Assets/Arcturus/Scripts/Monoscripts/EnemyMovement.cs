using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    public PlayerMovement enemyTarget;
    public Transform movementTarget;
    [Header("Components")]
    public Rigidbody2D rb2d;
    [SerializeField] protected SpriteRenderer spriteRend;
    public Enemy enemy;

    [Header("Movement Stats")]
    public float acceleration;
    public float friction;
    // variables to help with movement
    protected float distance;
    protected bool hasKnockback;
    public float knockbackTime;
    public LayerMask hitLayer;

    [Header("PathFinding")]
    public Node currentNode;
    public Node movedNode;
    public List<Node> path;
    public enum StateMachine
    {
        patrol,engage,evade
    }
    public StateMachine currentState;

    [Header("Attack stats")]
    protected bool canAttack = true, isReadyingAttack, isAttacking;
    public float AttackRange;
    public float stopRange;

    //other
    protected float colliderPushForce = 8;

    protected virtual void Start()
    {
        rb2d.linearDamping = friction;
        enemy = new Enemy(10,20,5,2,3);
        currentState = StateMachine.patrol;
    }
    protected virtual void Update()
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
        else if(enemyTarget != null && currentState != StateMachine.engage && enemy.currentHealth >= enemy.maxHealth * 20/100)
        {
            currentState = StateMachine.engage;
            path.Clear();
        }
        else if(enemyTarget != null && currentState != StateMachine.evade && enemy.currentHealth <= enemy.maxHealth * 20/100)
        {
            currentState = StateMachine.evade;
            path.Clear();
        }
        CreatePath();
        movedNode = AStarManager.instance.FindNearestNode(transform.position);
    }
    protected virtual void FixedUpdate()
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
            if(distance > stopRange && hit)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemy.topSpeed);
                rb2d.linearVelocity = velocity;
            }
            if(distance > stopRange && !hit && path.Count > 0)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(new Vector2(path[0].transform.position.x,path[0].transform.position.y))*acceleration;
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
        if(enemyTarget == null)
        {
            if(path.Count > 0)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(new Vector2(path[0].transform.position.x,path[0].transform.position.y))*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemy.topSpeed);
                rb2d.linearVelocity = velocity;
            }
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.rb2d.AddForce(TargetDirection(pm.transform.position)*colliderPushForce);
        }
        else if (collision.CompareTag("Enemy"))
        {
            EnemyMovement em = collision.GetComponent<EnemyMovement>();
            em.rb2d.AddForce(TargetDirection(em.transform.position)*colliderPushForce);
        }
    }
    public void Die()
    {
        Destroy(transform.parent.gameObject);
    }
    public void GetHit(PlayerMovement attacker, float knockback, float damage)
    {
        StartCoroutine(GetHitTimer());
        enemy.TakeDamage(damage);
        if(enemy.currentHealth <= 0)
            Die();
        rb2d.AddForce(attacker.DirectionToVector()*knockback,ForceMode2D.Impulse);
    }

    protected virtual IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150,0,0,255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(90,215,0,255);
        hasKnockback = false;
    }
    protected virtual IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        yield return new WaitForSeconds(0.3f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        isAttacking = true; // is now attacking
        yield return new WaitForSeconds(0.5f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(enemy.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    //movement help methods

    protected void CreatePath()
    {
        if(path.Count > 0)
        {
            if(movedNode != currentNode && movedNode != path[0] && currentState == StateMachine.engage)
            {
                path = AStarManager.instance.GeneratePath(movedNode, AStarManager.instance.FindNearestNode(enemyTarget.transform.position));
            }
            if(path.Count > 0)
            {
                if(Vector2.Distance(transform.position, path[0].transform.position) < 0.1f)
                {
                    currentNode = path[0];
                    path.RemoveAt(0);
                }
            }
            
        }
    }
    protected void Patrol()
    {
        if(path.Count == 0)
        {
            path = AStarManager.instance.GeneratePath(currentNode,AStarManager.instance.NodesInScene()[UnityEngine.Random.Range(0,AStarManager.instance.NodesInScene().Length)]);
        }
    }
    protected void Engage()
    {
        if(path.Count == 0)
        {
            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.FindNearestNode(enemyTarget.transform.position));
        }
    }
    protected void Evade()
    {
        if(path.Count == 0)
        {
            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.FindFarthestNode(enemyTarget.transform.position));
        }
    }
    
    public float TargetDistance(Vector2 playerPos)
    {
        return Vector2.Distance((Vector2)transform.position, playerPos);
    }
    public Vector2 TargetDirection(Vector2 targetPos)
    {
        return (targetPos - (Vector2)transform.position).normalized;
    }
    // get statements
    public bool IsAttacking()
    {
        return isAttacking;
    }
    public float GetColliderPushForce()
    {
        return colliderPushForce;
    }
    public void FlipSpriteRend(bool yesorno)
    {
        spriteRend.flipX = yesorno;
    }
    public void setVelocity(Vector2 velocity)
    {
        rb2d.linearVelocity = velocity;
    }
}
