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

    public Node currentNode;
    public List<Node> path = new List<Node>();

    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemy = new Enemy(10,20,5,2,2);
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
            if(distance > stopRange && hit)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemy.topSpeed);
                rb2d.linearVelocity = velocity;
            }
            if (distance > stopRange && !hit)
            {
                CreatePath();
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

    public void CreatePath()
    {
        if(path.Count > 0)
        {
            int x = 0;
            transform.position= Vector2.MoveTowards(transform.position, new Vector2(path[x].transform.position.x,path[x].transform.position.y),3*Time.deltaTime);

            if(Vector2.Distance(transform.position,path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
        else
        {
            Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
            while(path == null || path.Count == 0)
            {
                if(enemyTarget == null)
                    path = AStarManager.instance.GeneratePath(currentNode, nodes[UnityEngine.Random.Range(0,nodes.Length)]);
                else
                {
                    path = AStarManager.instance.GeneratePath(currentNode, enemyTarget.currentNode);
                }
            }
        }
    }
}
