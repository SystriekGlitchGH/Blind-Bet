using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;


public class Goop1Movement : EnemyMovement
{
    public float dashForce;
    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemyStats = new Enemy(10,20,5,2,1.25f);
        currentState = StateMachine.patrol;
    }
    protected override void Update()
    {
        switch (currentState)
        {
            case StateMachine.patrol:
                Patrol();
                break;
            case StateMachine.engage:
                Engage();
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
        CreatePath();
        movedNode = AStarManager.instance.FindNearestNode(transform.position);
        enemyStats.CheckEffects();
        if (enemyStats.effectManager.effects.Count != 0)
        {
            for (int i = 0; i < enemyStats.effectManager.effects.Count; i++)
            {
                enemyStats.effectManager.effects[i].elapsedTime += Time.deltaTime;
                if (enemyStats.effectManager.effects[i].elapsedTime >= enemyStats.effectManager.effects[i].duration)
                {
                    enemyStats.effectManager.effects.Remove(enemyStats.effectManager.effects[i]);
                }
            }
        }
        CheckCurrentColor();
    }
    protected override void FixedUpdate()
    {
        
        if (enemyTarget != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position,TargetDirection(enemyTarget.transform.position),3,hitLayer);
            Debug.DrawRay(rb2d.position, TargetDirection(enemyTarget.transform.position) * 3f, Color.red);
            if (hasKnockback || isAttacking)
            {
                return;
            }
            if(distance < AttackRange && canAttack && hit)
            {
                StartCoroutine(AttackTimer("enemyTarget"));
            }
            if(distance < AttackRange && canAttack && !hit && path.Count > 0)
            {
                StartCoroutine(AttackTimer("node"));
            }
            if(distance < 2 && !isAttacking)
            {
                rb2d.linearDamping = friction;
            }
            if(distance > 40)
            {
                enemyTarget = null;
                rb2d.linearDamping = friction;
            }
        }
        if(enemyTarget == null && canAttack)
        {
            StartCoroutine(AttackTimer("node"));
        }
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttacking)
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.GetHit(this,enemyStats.baseKnockback, enemyStats.baseDamage * enemyStats.GetAttackDamageMod());
        }
    }

    protected IEnumerator AttackTimer(string targetArea)
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(210,225,0,255);
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = currentColor;
        isAttacking = true; // is now attacking
        if(targetArea == "enemyTarget")
        {
            rb2d.AddForce(TargetDirection(enemyTarget.transform.position)*dashForce, ForceMode2D.Impulse);
        }
        else if(targetArea == "node")
        {
            rb2d.AddForce(TargetDirection(new Vector2(path[0].transform.position.x,path[0].transform.position.y))*dashForce, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(enemyStats.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
}
