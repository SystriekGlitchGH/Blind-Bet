using System.Collections;
using UnityEngine;

public class GoopThatShoot1Movement : EnemyMovement
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform anchorTransform;
    protected override void Start()
    {
        rb2d.linearDamping = friction;
        enemy = new Enemy(10,20,5,2,2);
        currentState = StateMachine.patrol;
    }
    protected override void Update()
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
    } 
    protected override IEnumerator AttackTimer()
    {
        canAttack = false; // make the enemy not duplicate attacks
        isReadyingAttack = true; // small moment before attack to make it not instant
        spriteRend.color = new Color32(210,225,0,255);
        yield return new WaitForSeconds(0.5f); // amount of time to react to attack
        isReadyingAttack = false; // no longer readying attack
        spriteRend.color = new Color32(0,160,225,255);
        isAttacking = true; // is now attacking
        SpawnBullet();
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(enemy.attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }
    protected override IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150, 0, 0, 255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(0, 160, 225, 255);
        hasKnockback = false;
    }

    private void SpawnBullet()
    {
        GameObject shot = Instantiate(bullet, transform.position + (Vector3)TargetDirection(enemyTarget.transform.position), anchorTransform.rotation);
        if (shot.TryGetComponent(out Bullet bt))
        {
            bt.bulletType = "enemy";
            bt.em = this;
            bt.direction = TargetDirection(enemyTarget.transform.position);
            bt.rb2d.AddForce(bt.rb2d.transform.up * 750);
        }
    }
}
