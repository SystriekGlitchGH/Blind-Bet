using System.Collections;
using UnityEngine;

public class CardSoliderD : EnemyMovement
{
    public GameObject attackVisual;
    [SerializeField] Transform anchorTransform;
    public LayerMask boxLayer;
    public Vector2 attackSize;
    protected bool isRetreating;
    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemy = new Enemy(10,30,6,2,3);
    }
    protected void Update()
    {
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
        if(enemy.currentHealth <= enemy.maxHealth * 0.25)
            isRetreating = true;
        else if(enemy.currentHealth >= enemy.maxHealth * 0.75)
            isRetreating = false;
    }
    protected override void FixedUpdate()
    {
        if(enemyTarget != null)
        {
            if (hasKnockback || isAttacking)
            {
                return;
            }
            distance = TargetDistance(enemyTarget.transform.position);
            if(distance > stopRange && !isRetreating)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(enemyTarget.transform.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemy.topSpeed);
                rb2d.linearVelocity = velocity;
            }
            if (isRetreating)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(enemyTarget.transform.position)*acceleration;
                rb2d.AddForce(-newVelocity);
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
            rb2d.AddForce(-TargetDirection(em.transform.position)*colliderPushForce);
        }
    }
    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(210,225,0,255);
        yield return new WaitForSeconds(0.3f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = new Color32(225,0,150,255);
        isAttacking = true; // is now attacking

        Debug.Log("Enemy attacked");
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * anchorTransform.rotation.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * anchorTransform.rotation.eulerAngles.z));
        Vector2 position = angleAsVector * (attackSize.y/2+1);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + (Vector3)position, attackSize, anchorTransform.rotation.z, Vector2.zero,0,boxLayer);
        if(hit && hit.rigidbody.TryGetComponent(out PlayerMovement player))
        {
            player.GetHit(this, enemy.baseKnockback);
        }
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
        attack.transform.localScale = attackSize;

        yield return new WaitForSeconds(0.1f); // time where you can take damage/parry/get shot at
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
        spriteRend.color = new Color32(225, 0, 150, 255);
        hasKnockback = false;
    }
}
