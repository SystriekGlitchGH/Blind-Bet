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
        idle,attack,dash,red,white,blue
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
    private bool inWhiteAttack, canWhiteAttack = true;
    private bool foundPositionWhite;
    private LineRenderer lr1, lr2, lr3, lr4;
    public LineRenderer lineRend;
    public LayerMask beamLayer;
    private float elapsedTime;
    // blue
    public GameObject specterBullet;
    private bool foundPositionBlue;
    private bool inBlueAttack, canBlueAttack = true;

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
        if (inWhiteAttack)
        {
            ActivateWhite();
            elapsedTime += Time.deltaTime;
        }
            
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
            if(state == BossStates.blue)
            {
                if (!foundPositionBlue)
                {
                    int positionNum = rand.Next(1,5);
                    if(positionNum == 1)
                        movementTarget = topRightPos;
                    else if(positionNum == 2)
                        movementTarget = topLeftPos;
                    else if(positionNum == 3)
                        movementTarget = bottomRightPos;
                    else if(positionNum == 4)
                        movementTarget = bottomLeftPos;
                    foundPositionBlue = true;
                }
                if (foundPositionBlue && Vector2.Distance(transform.position, movementTarget.position) > 0.5f)
                {
                    rb2d.linearDamping = 0;
                    Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                    rb2d.AddForce(newVelocity);
                    Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                    rb2d.linearVelocity = velocity;
                }
                if(Vector2.Distance(transform.position, movementTarget.position) < 0.5f && canBlueAttack)
                {
                    rb2d.linearDamping = friction;
                    StartCoroutine(BlueTimer());
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
        yield return new WaitForSeconds(1);
        inWhiteAttack = true;
        lr1 = Instantiate(lineRend);
        lr2 = Instantiate(lineRend);
        lr3 = Instantiate(lineRend);
        lr4 = Instantiate(lineRend);
        yield return new WaitForSeconds(5);
        Destroy(lr1.gameObject);
        Destroy(lr2.gameObject);
        Destroy(lr3.gameObject);
        Destroy(lr4.gameObject);
        elapsedTime = 0;
        inWhiteAttack = false;
        SwitchState();
        canWhiteAttack = true;
        foundPositionWhite = false;
    }
    protected void ActivateWhite()
    {
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad*(elapsedTime*90)), Mathf.Cos(Mathf.Deg2Rad*(elapsedTime*90)));
        RaycastHit2D[] hits1 = Physics2D.RaycastAll(anchorTransform.position + (Vector3)angleAsVector*2, angleAsVector,50,beamLayer);
        lr1.SetPosition(0, anchorTransform.position + (Vector3)angleAsVector*2);
        foreach(RaycastHit2D hit in hits1)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
                {
                    player.GetHit(this, 0.1f,enemyStats.baseDamage*enemyStats.GetAttackDamageMod());
                }
                continue;
            }
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                lr1.SetPosition(1, hit.point);
                break;
            }
        }

        angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * (90+elapsedTime*90)), Mathf.Cos(Mathf.Deg2Rad * (90+elapsedTime*90)));
        RaycastHit2D[] hits2 = Physics2D.RaycastAll(anchorTransform.position + (Vector3)angleAsVector*2, angleAsVector,50,beamLayer);
        lr2.SetPosition(0, anchorTransform.position + (Vector3)angleAsVector*2);
        foreach(RaycastHit2D hit in hits2)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
                {
                    player.GetHit(this, 0.1f,enemyStats.baseDamage*enemyStats.GetAttackDamageMod());
                }
                continue;
            }
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                lr2.SetPosition(1, hit.point);
                break;
            }
        }

        angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * (180+elapsedTime*90)), Mathf.Cos(Mathf.Deg2Rad * (180+elapsedTime*90)));
        RaycastHit2D[] hits3 = Physics2D.RaycastAll(anchorTransform.position + (Vector3)angleAsVector*2, angleAsVector,50,beamLayer);
        lr3.SetPosition(0, anchorTransform.position + (Vector3)angleAsVector*2);
        foreach(RaycastHit2D hit in hits3)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
                {
                    player.GetHit(this, 0.1f,enemyStats.baseDamage*enemyStats.GetAttackDamageMod());
                }
                continue;
            }
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                lr3.SetPosition(1, hit.point);
                break;
            }
        }

        angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * (270+elapsedTime*90)), Mathf.Cos(Mathf.Deg2Rad * (270+elapsedTime*90)));
        RaycastHit2D[] hits4 = Physics2D.RaycastAll(anchorTransform.position + (Vector3)angleAsVector*2, angleAsVector,50,beamLayer);
        lr4.SetPosition(0, anchorTransform.position + (Vector3)angleAsVector*2);
        foreach(RaycastHit2D hit in hits4)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
                {
                    player.GetHit(this, 0.1f,enemyStats.baseDamage*enemyStats.GetAttackDamageMod());
                }
                continue;
            }
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                lr4.SetPosition(1, hit.point);
                break;
            }
        }
    }
    protected IEnumerator BlueTimer()
    {
        canBlueAttack = false;
        float extraRotation = -90 / 2;
        for(int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(1f);
            for (int j = 0; j < 6+i; j++)
            {
                Vector3 rotation = anchorTransform.rotation.eulerAngles + new Vector3(0, 0, extraRotation);
                GameObject shot = Instantiate(specterBullet, transform.position + (Vector3)TargetDirection(enemyTarget.transform.position), Quaternion.Euler(rotation));
                if (shot.TryGetComponent(out Bullet b))
                {
                    b.bulletType = "enemy";
                    b.em = this;
                    b.direction = (Vector3)TargetDirection(enemyTarget.transform.position);
                    b.rb2d.AddForce(b.rb2d.transform.up * 1000);
                }
                extraRotation += 90 / (6+i-1);
            }
            extraRotation = -90 / 2;
        }
        SwitchState();
        foundPositionBlue = false;
        canBlueAttack = true;
    }
    protected void SwitchState()
    {
        int attackNum = rand.Next(1,6);
        if(attackNum == 1)
            state = BossStates.attack;
        if(attackNum == 2)
            state = BossStates.dash;
        if(attackNum == 3)
            state = BossStates.red;
        if(attackNum == 4)
            state = BossStates.white;
        if(attackNum == 5)
            state = BossStates.blue;
    }
}
