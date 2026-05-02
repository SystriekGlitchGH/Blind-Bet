using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;
public class Area1Boss : EnemyMovement
{
    [Header("Boss Positions")]
    [SerializeField] Transform centerPos;
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;
    [SerializeField] Transform topPos;
    [SerializeField] Transform bottomPos;
    [SerializeField] Transform topRightPos;
    [SerializeField] Transform topLeftPos;
    [SerializeField] Transform bottomRightPos;
    [SerializeField] Transform bottomLeftPos;
    public enum BossStates
    {
        idle,attack,dash,red,white,blue,green,black,purple
    }
    public BossStates state;
    public Random rand = new Random();

    public GameObject attackVisual;
    [SerializeField] Transform anchorTransform;
    [Header("Attack Stats")]
    public Vector2 attackSize;
    // dashing
    private bool isDashing;
    private bool canDash = true;
    public float dashRange;
    public float dashLength;
    // red
    public GameObject fireball;
    private bool foundPositionRed;
    private bool inRedAttack, canRedAttack = true;
    // white
    private bool inWhiteAttack, canWhiteAttack;
    private bool foundPositionWhite;

    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemyStats = new Enemy(10,500,5,5,0);
    }
    protected override void Update()
    {
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

        if(enemyTarget != null)
        {
            // find the angle from a normalised vector2
            float angleRadians = Mathf.Atan2(TargetDirection(enemyTarget.transform.position).y,TargetDirection(enemyTarget.transform.position).x);
            //converts that angle to degrees, not radians
            float angleDegrees = angleRadians * Mathf.Rad2Deg; 
            angleDegrees -= 90; // sets the rotation correctly by 90 degrees
            //anchorTransform.rotation = Quaternion.LookRotation(PlayerDirection(target.transform.position));
            anchorTransform.rotation = Quaternion.Euler(0,0,angleDegrees);
        }
        if(inWhiteAttack)
            ActivateWhite();
    }
    protected override void FixedUpdate()
    {
        if(enemyTarget != null)
        {
            if (hasKnockback || isAttacking || enemyStats.hasStun || enemyStats.hasFrozen || state == BossStates.idle)
            {
                return;
            }
            distance = TargetDistance(enemyTarget.transform.position);
            if(state == BossStates.attack)
            {
                if(distance > attackRange)
                {
                    movementTarget = enemyTarget.transform;
                    rb2d.linearDamping = 0;
                    Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                    rb2d.AddForce(newVelocity);
                    Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                    rb2d.linearVelocity = velocity;
                }
                if(distance <= attackRange && canAttack)
                {
                    StartCoroutine(AttackTimer());
                }
            }
            if(state == BossStates.dash)
            {
                if(distance > dashRange && !isDashing)
                {
                    movementTarget = enemyTarget.transform;
                    rb2d.linearDamping = 0;
                    Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                    rb2d.AddForce(newVelocity);
                    Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                    rb2d.linearVelocity = velocity;
                }
                if(distance <= dashRange && canDash)
                {
                    StartCoroutine(DashTimer());
                }
            }
            if(state == BossStates.red)
            {
                if (!foundPositionRed)
                {
                    int positionNum = rand.Next(1,5);
                    if(positionNum == 1)
                        movementTarget = topPos;
                    else if(positionNum == 2)
                        movementTarget = rightPos;
                    else if(positionNum == 3)
                        movementTarget = bottomPos;
                    else if(positionNum == 4)
                        movementTarget = leftPos;
                    foundPositionRed = true;
                }
                if (foundPositionRed && Vector2.Distance(transform.position, movementTarget.position) > 0.5f)
                {
                    rb2d.linearDamping = 0;
                    Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                    rb2d.AddForce(newVelocity);
                    Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                    rb2d.linearVelocity = velocity;
                }
                if(Vector2.Distance(transform.position, movementTarget.position) < 0.5f && canRedAttack)
                {
                    rb2d.linearDamping = friction;
                    StartCoroutine(RedTimer());
                }
            }
            if(state == BossStates.white)
            {
                if (!foundPositionWhite)
                {
                    movementTarget = centerPos;
                    foundPositionWhite = true;
                }
                if (foundPositionWhite && Vector2.Distance(transform.position, movementTarget.position) > 0.5f)
                {
                    rb2d.linearDamping = 0;
                    Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                    rb2d.AddForce(newVelocity);
                    Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                    rb2d.linearVelocity = velocity;
                }
                if(Vector2.Distance(transform.position, movementTarget.position) < 0.5f && canWhiteAttack)
                {
                    rb2d.linearDamping = friction;
                    StartCoroutine(WhiteTimer());
                }
            }
            if(distance < stopRange && !isDashing)
            {
                rb2d.linearDamping = friction;
            }
        }   
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
        if (isDashing)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerMovement pm = collision.GetComponent<PlayerMovement>();
                pm.GetHit(this, enemyStats.baseKnockback, enemyStats.baseDamage * enemyStats.GetAttackDamageMod());
            }
        }
    }
    // attacks
    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // is now attacking
        spriteRend.color = new Color32(210,225,0,255);
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        spriteRend.color = currentColor;
        isAttacking = true;

        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * anchorTransform.rotation.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * anchorTransform.rotation.eulerAngles.z));
        Vector2 position = angleAsVector * (attackSize.y/2+1);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + (Vector3)position, attackSize, anchorTransform.rotation.z, Vector2.zero,0,hitLayer);
        if(hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
            player.GetHit(this, enemyStats.baseKnockback, enemyStats.baseDamage * enemyStats.GetAttackDamageMod());
        
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
        attack.transform.localScale = attackSize;

        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        Destroy(attack);
        isAttacking = false;
        yield return new WaitForSeconds(1f);
        SwitchState();
        canAttack = true; // no longer attacking
    }
    protected IEnumerator DashTimer()
    {
        canDash = false; // is now attacking
        spriteRend.color = new Color32(210,225,0,255);
        rb2d.AddForce(-TargetDirection(movementTarget.position)*10,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        spriteRend.color = currentColor;
        isDashing = true;
        rb2d.linearDamping = 0;
        rb2d.AddForce(TargetDirection(movementTarget.position)*dashLength,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f); // time where you can take damage/parry/get shot at
        isDashing = false;
        yield return new WaitForSeconds(1f);
        SwitchState();
        canDash = true;

    }
    protected IEnumerator RedTimer()
    {
        canRedAttack = false;
        float extraRotation = -160 / 2;
        Vector2 targetDirection = TargetDirection(enemyTarget.transform.position);
        
        float angleRadians = Mathf.Atan2(TargetDirection(enemyTarget.transform.position).y,TargetDirection(enemyTarget.transform.position).x);
        //converts that angle to degrees, not radians
        float angleDegrees = angleRadians * Mathf.Rad2Deg; 
        angleDegrees -= 90; // sets the rotation correctly by 90 degrees
        //anchorTransform.rotation = Quaternion.LookRotation(PlayerDirection(target.transform.position));
        Quaternion targetRotation = Quaternion.Euler(0,0,angleDegrees);
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Vector3 rotation = targetRotation.eulerAngles + new Vector3(0, 0, extraRotation);
            GameObject shot = Instantiate(fireball, transform.position + (Vector3)targetDirection, Quaternion.Euler(rotation));
            if (shot.TryGetComponent(out Bullet b))
            {
                b.bulletType = "enemy";
                b.em = this;
                b.direction = (Vector3)TargetDirection(targetDirection);
                b.rb2d.AddForce(b.rb2d.transform.up * 1000);
            }
            extraRotation += 160 / (20-1);
        }
        SwitchState();
        foundPositionRed = false;
        canRedAttack = true;
    }
    protected IEnumerator WhiteTimer()
    {
        canWhiteAttack = false;
        inWhiteAttack = true;
        yield return new WaitForSeconds(5);
        inWhiteAttack = false;
        canWhiteAttack = true;
        foundPositionWhite = false;
    }
    protected void ActivateWhite()
    {
        
    }
    protected void SwitchState()
    {
        int attackNum = rand.Next(1,4);
        if(attackNum == 1)
            state = BossStates.attack;
        if(attackNum == 2)
            state = BossStates.dash;
        if(attackNum == 3)
            state = BossStates.red;
        // if(attackNum == 4)
        //     state = BossStates.white;
        // if(attackNum == 5)
        //     state = BossStates.blue;
        // if(attackNum == 6)
        //     state = BossStates.green;
        // if(attackNum == 7)
        //     state = BossStates.black;
        // if(attackNum == 8)
        //     state = BossStates.purple;
    }
}
