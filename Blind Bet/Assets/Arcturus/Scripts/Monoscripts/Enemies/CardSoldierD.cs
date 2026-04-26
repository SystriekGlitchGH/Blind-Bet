using System.Collections;
using UnityEngine;

public class CardSoldierD : EnemyMovement
{
    public GameObject attackVisual;
    [SerializeField] Transform anchorTransform;
    public LayerMask boxLayer;
    public Vector2 attackSize;
    protected override void Start()
    {
        currentNode = AStarManager.instance.FindNearestNode(transform.position);
        rb2d.linearDamping = friction;
        enemyStats = new Enemy(8,50,6,2,3);
        currentState = StateMachine.patrol;
    }
    protected override void Update()
    {
        base.Update();
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
    }
    protected override void FixedUpdate()
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
            if(distance > stopRange && hit && currentState == StateMachine.engage)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                rb2d.linearVelocity = velocity;
            }
            if(distance > stopRange && hit && currentState == StateMachine.evade)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                rb2d.AddForce(-newVelocity);
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
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.rb2d.AddForce(TargetDirection(pm.transform.position)*colliderPushForce);
        }
        if (collision.CompareTag("Soldier"))
        {
           rb2d.AddForce(-TargetDirection(collision.transform.position) * colliderPushForce*3);
        }
        if (collision.CompareTag("Enemy"))
        {
            EnemyMovement em = collision.GetComponent<EnemyMovement>();
            em.rb2d.AddForce(TargetDirection(em.transform.position)*colliderPushForce);
        }
    }
    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(210,225,0,255);
        yield return new WaitForSeconds(0.3f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = currentColor;
        isAttacking = true; // is now attacking

        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * anchorTransform.rotation.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * anchorTransform.rotation.eulerAngles.z));
        Vector2 position = angleAsVector * (attackSize.y/2+1);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + (Vector3)position, attackSize, anchorTransform.rotation.z, Vector2.zero,0,boxLayer);
        if(hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
            player.GetHit(this, enemyStats.baseKnockback, enemyStats.baseDamage * enemyStats.GetAttackDamageMod());
        
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
        attack.transform.localScale = attackSize;

        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        Destroy(attack);
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(enemyStats.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
}
