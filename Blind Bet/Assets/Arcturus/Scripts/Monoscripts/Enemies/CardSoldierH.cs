using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class CardSoldierH : EnemyMovement
{
    public GameObject healVisual;
    public LayerMask circleLayer;
    public float healRadius;
    public float healPercent;
    private bool canHeal = true;
    public float healCooldown;
    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemyStats = new Enemy(10,20,6,2,3);
        currentState = StateMachine.patrol;
        isHealer = true;
    }
    protected override void FixedUpdate()
    {
        if(enemyTarget != null)
        {
            if (hasKnockback)
            {
                return;
            }
            distance = TargetDistance(enemyTarget.transform.position);
            if(distance > stopRange && path.Count > 0)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(new Vector2(path[0].transform.position.x,path[0].transform.position.y))*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed);
                rb2d.linearVelocity = velocity;
            }
            if(canHeal)
            {
                StartCoroutine(HealTimer());
            }
            if(distance < stopRange)
            {
                rb2d.linearDamping = friction;
                if(distance < stopRange - 1)
                {
                    rb2d.AddForce(-TargetDirection(enemyTarget.transform.position)*acceleration/4);
                    Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed);
                    rb2d.linearVelocity = velocity;
                }
                
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
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed);
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
    protected IEnumerator HealTimer()
    {
        canHeal = false; // make the enemy not duplicate attacks
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position,healRadius,Vector2.zero, 0, circleLayer);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHealed(enemy.enemyStats.maxHealth*(healPercent/100));
                Debug.Log(enemy.enemyStats.currentHealth);
            }
        }
        GameObject attack = Instantiate(healVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale *= healRadius*2;

        yield return new WaitForSeconds(0.1f); // time to show heal circle
        Destroy(attack);
        yield return new WaitForSeconds(healCooldown-0.1f); // cooldown so the enemies don't spam attacks
        canHeal = true; // can attack again
    }
    protected override IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150, 0, 0, 255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(225, 0, 150, 255);
        hasKnockback = false;
    }
    public override IEnumerator GetHealedTimer()
    {
        spriteRend.color = new Color32(0,150,0,255);
        yield return new WaitForSeconds(0.1f);
        spriteRend.color = new Color32(225, 0, 150, 255);
    }
}
