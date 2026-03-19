using System.Collections;
using UnityEngine;

public class GoopThatShoot1Movement : EnemyMovement
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform anchorTransform;
    private void Update()
    {
        if(target != null)
        {
            // find the angle from a normalised vector2
            float angleRadians = Mathf.Atan2(PlayerDirection(target.transform.position).y,PlayerDirection(target.transform.position).x);
            //converts that angle to degrees, not radians
            float angleDegrees = angleRadians * Mathf.Rad2Deg; 
            angleDegrees -= 90; // sets the rotation correctly by 90 degrees
            //anchorTransform.rotation = Quaternion.LookRotation(PlayerDirection(target.transform.position));
            anchorTransform.rotation = Quaternion.Euler(0,0,angleDegrees);
        }
        
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
        Debug.Log("Enemy attacked");
        yield return new WaitForSeconds(0.2f); // time where you can take damage/parry/get shot at
        isAttacking = false; // no longer attacking
        yield return new WaitForSeconds(attackCooldown); // cooldown so the enemies don't spam attacks
        canAttack = true; // can attack again
    }

    private void SpawnBullet()
    {
        GameObject shot = Instantiate(bullet, transform.position + (Vector3)PlayerDirection(target.transform.position), anchorTransform.rotation);
        if (shot.TryGetComponent(out Bullet bt))
        {
            bt.bulletType = "enemy";
            bt.em = this;
            bt.direction = PlayerDirection(target.transform.position);
            bt.rb2d.AddForce(bt.rb2d.transform.up * 1000);
        }
    }
}
