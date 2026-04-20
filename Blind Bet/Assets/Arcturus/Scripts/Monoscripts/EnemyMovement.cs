using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    public PlayerMovement enemyTarget;
    public EnemyMovement healerTarget;
    public Transform movementTarget;
    [Header("Components")]
    public Rigidbody2D rb2d;
    [SerializeField] protected SpriteRenderer spriteRend;
    public Enemy enemyStats;

    [Header("Movement Stats")]
    public float acceleration;
    public float friction;
    // variables to help with movement
    protected float distance;
    public bool hasKnockback;
    public float knockbackTime;
    public LayerMask hitLayer;

    [Header("Feedback Colors")]
    public Color32 baseColor;
    public Color32 currentColor;

    [Header("PathFinding")]
    public Node currentNode;
    public Node movedNode;
    public List<Node> path;
    public enum StateMachine
    {
        patrol,engage,evade
    }
    public StateMachine currentState;
    protected bool isRetreating;

    [Header("Attack stats")]
    protected bool canAttack = true, isReadyingAttack, isAttacking;
    public float AttackRange;
    public float stopRange;
    protected bool canGetPoison = true;

    //other
    protected float colliderPushForce = 8;
    public bool isHealer;

    protected virtual void Start()
    {
        rb2d.linearDamping = friction;
        enemyStats = new Enemy(10,20,5,2,3);
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
        else if(enemyTarget != null && currentState != StateMachine.engage && enemyStats.currentHealth > enemyStats.maxHealth * 20/100 && !isRetreating)
        {
            currentState = StateMachine.engage;
            path.Clear();
        }
        else if(enemyTarget != null && currentState != StateMachine.evade && enemyStats.currentHealth <= enemyStats.maxHealth * 20/100)
        {
            currentState = StateMachine.evade;
            isRetreating = true;
            path.Clear();
        }
        CreatePath();
        movedNode = AStarManager.instance.FindNearestNode(transform.position);
        if (isRetreating)
        {
            if(enemyStats.currentHealth >= enemyStats.maxHealth * 0.75)
            {
                isRetreating = false;
            }
        }
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
    protected virtual void FixedUpdate()
    {
        if(enemyTarget != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position,TargetDirection(enemyTarget.transform.position),3,hitLayer);
            Debug.DrawRay(rb2d.position, TargetDirection(enemyTarget.transform.position) * 3f, Color.red);
            if (hasKnockback || isAttacking || enemyStats.hasStun || enemyStats.hasFrozen)
            {
                return;
            }
            distance = TargetDistance(enemyTarget.transform.position);
            if(distance > stopRange && hit)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                rb2d.linearVelocity = velocity;
            }
            if(distance > stopRange && !hit && path.Count > 0)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(new Vector2(path[0].transform.position.x,path[0].transform.position.y))*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
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
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
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
    #region ACTIVATION METHODS
    public void Die()
    {
        enemyTarget.playerStats.kills++;
        Destroy(transform.parent.gameObject);
    }
    public void GetHit(PlayerMovement attacker, float knockback, float damage)
    {
        StartCoroutine(GetHitTimer());
        enemyStats.TakeDamage(damage * enemyStats.GetDamageMod());
        Debug.Log(enemyStats.currentHealth);
        if (enemyStats.currentHealth <= 0)
            Die();
        rb2d.AddForce(attacker.DirectionToVector()*knockback,ForceMode2D.Impulse);
    }
    public void GetHitAway(PlayerMovement attacker, float knockback, float damage)
    {
        StartCoroutine(GetHitTimer());
        enemyStats.TakeDamage(damage * enemyStats.GetDamageMod());
        Debug.Log(enemyStats.currentHealth);
        if(enemyStats.currentHealth <= 0)
            Die();
        rb2d.AddForce(-TargetDirection(attacker.transform.position)*knockback,ForceMode2D.Impulse);
    }
    public IEnumerator GetHitDelay(PlayerMovement attacker, float knockback, float damage, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(GetHitTimer());
        enemyStats.TakeDamage(damage * enemyStats.GetDamageMod());
        Debug.Log(enemyStats.currentHealth);
        if(enemyStats.currentHealth <= 0)
            Die();
        rb2d.AddForce(-TargetDirection(attacker.transform.position)*knockback,ForceMode2D.Impulse);
    }
    public void GetHit(Bullet bullet, float knockback, float damage)
    {
        StartCoroutine(GetHitTimer());
        enemyStats.TakeDamage(damage * enemyStats.GetDamageMod());
        if(enemyStats.currentHealth <= 0)
            Die();
        rb2d.AddForce(-TargetDirection(bullet.transform.position)*knockback,ForceMode2D.Impulse);
    }
    public void GetHealed(float amount)
    {
        StartCoroutine(GetHealedTimer());
        enemyStats.Heal(amount);
    }
    #endregion
    #region IENUMERATORS
    public virtual IEnumerator GetHealedTimer()
    {
        spriteRend.color = new Color32(0,150,0,255);
        yield return new WaitForSeconds(0.1f);
        spriteRend.color = currentColor;
    }
    protected virtual IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150,0,0,255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = currentColor;
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
        yield return new WaitForSeconds(enemyStats.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    protected IEnumerator PoisonTimer()
    {
        canGetPoison = false;
        enemyStats.TakeDamage(5);
        spriteRend.color = new Color32(0,120,0,255);
        yield return new WaitForSeconds(0.1f);
        spriteRend.color = currentColor;
        yield return new WaitForSeconds(1f);
        canGetPoison = true;

    }
    #endregion
    protected void CheckCurrentColor()
    {
        Color32 setColor = baseColor;
        if (enemyStats.hasCharm)
            setColor = CombineColors(setColor, new Color32(255,70,190,255));
        if (enemyStats.hasChill)
            setColor = CombineColors(setColor, new Color32(80,190,255,255));
        if (enemyStats.hasFrozen)
            setColor = CombineColors(setColor, new Color32(170, 220, 255, 255));
        if (enemyStats.hasPoison)
            setColor = CombineColors(setColor, new Color32(50, 220, 70, 255));
        if(enemyStats.hasEnrage)
            setColor = CombineColors(setColor, new Color32(240, 100, 0, 255));
        currentColor = setColor;
    }
    protected Color32 CombineColors(Color32 color1, Color32 color2)
    {
        int r = (color1.r + color2.r) / 2;
        int g = (color1.g + color2.g) / 2;
        int b = (color1.b + color2.b) / 2;
        return new Color32((byte)r, (byte)g, (byte)b, 255);
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
            if(healerTarget == null)
            {
                path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.FindFarthestNode(enemyTarget.transform.position));
            }
            else
            {
                path = AStarManager.instance.GeneratePath(currentNode, healerTarget.movedNode);
            }
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
