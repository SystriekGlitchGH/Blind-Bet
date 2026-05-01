using UnityEditor;
using UnityEngine;

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
    protected override void Start()
    {
        state = BossStates.attack;
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
            if(distance > stopRange && state == BossStates.attack)
            {
                movementTarget = enemyTarget.transform;
                rb2d.linearDamping = 0;
                Vector2 newVelocity = TargetDirection(movementTarget.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), enemyStats.topSpeed * enemyStats.GetSpeedMod());
                rb2d.linearVelocity = velocity;
            }
            if(distance < stopRange && !isAttacking)
            {
                rb2d.linearDamping = friction;
            }
        }   
    } 
}
