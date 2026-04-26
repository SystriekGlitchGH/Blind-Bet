using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ChargerMovement : EnemyMovement
{
    public float chargeAcceleration;
    public float chargeTime;
    protected override void Start()
    {
        currentNode = AStarManager.instance.FindNearestNode(transform.position);
        rb2d.linearDamping = friction;
        enemyStats = new Enemy(8, 50, 8, 2, 6);
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
        if (enemyTarget == null && currentState != StateMachine.patrol)
        {
            currentState = StateMachine.patrol;
            path.Clear();
        }
        else if (enemyTarget != null && currentState != StateMachine.engage)
        {
            currentState = StateMachine.engage;
            path.Clear();
        }
        CreatePath();
        movedNode = AStarManager.instance.FindNearestNode(transform.position);
        enemyStats.CheckEffects();
        if(enemyStats.effectManager.effects.Count != 0)
        {
            for(int i = 0; i < enemyStats.effectManager.effects.Count; i++)
            {
                enemyStats.effectManager.effects[i].elapsedTime += Time.deltaTime;
                if(enemyStats.effectManager.effects[i].elapsedTime >= enemyStats.effectManager.effects[i].duration)
                {
                    enemyStats.effectManager.effects.Remove(enemyStats.effectManager.effects[i]);
                    enemyStats.CheckEffects();
                    CheckCurrentColor();
                    spriteRend.color = currentColor;
                }
            }
        }
        if (enemyStats.hasPoison && canGetPoison)
        {
            StartCoroutine(PoisonTimer());
        }
        if(enemyStats.hasStun || enemyStats.hasFrozen)
            rb2d.linearVelocity = Vector2.zero;
        CheckCurrentColor();
    }
    protected override void FixedUpdate()
    {
        if (enemyTarget != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, TargetDirection(enemyTarget.transform.position), 3, hitLayer);
            Debug.DrawRay(rb2d.position, TargetDirection(enemyTarget.transform.position) * 3f, Color.red);
            if (hasKnockback || enemyStats.hasStun || enemyStats.hasFrozen)
                return;
            if (isAttacking)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position) * chargeAcceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                rb2d.linearVelocity = velocity;
                return;
            }
            distance = TargetDistance(enemyTarget.transform.position);
            if (distance > stopRange && hit)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position) * acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                rb2d.linearVelocity = velocity;
            }
            if (distance > stopRange && !hit && path.Count > 0)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(new Vector2(path[0].transform.position.x, path[0].transform.position.y)) * acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                rb2d.linearVelocity = velocity;
            }
            if (distance < AttackRange && canAttack)
            {
                StartCoroutine(AttackTimer());
            }
            if (distance < stopRange && !isAttacking)
            {
                rb2d.linearDamping = friction;
            }
            if (distance > 40)
            {
                enemyTarget = null;
                rb2d.linearDamping = friction;
            }
        }
        if (enemyTarget == null)
        {
            if (path.Count > 0)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(new Vector2(path[0].transform.position.x, path[0].transform.position.y)) * acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                rb2d.linearVelocity = velocity;
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
        spriteRend.color = currentColor;
        isAttacking = true; // is now attacking
        enemyStats.topSpeed = 6;
        yield return new WaitForSeconds(chargeTime); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        enemyStats.topSpeed = 2;
        yield return new WaitForSeconds(enemyStats.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttacking)
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.GetHit(this, enemyStats.baseKnockback, enemyStats.baseDamage * enemyStats.GetAttackDamageMod());
        }
    }
}
