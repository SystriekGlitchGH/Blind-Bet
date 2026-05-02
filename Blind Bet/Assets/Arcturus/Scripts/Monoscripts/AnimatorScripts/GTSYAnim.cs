using UnityEngine;

public class GTSYAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] EnemyMovement em;
    private void Update()
    {
        if(em.enemyTarget != null)
        {
            anim.SetBool("isShooting",em.IsAttacking());
            anim.SetBool("isReadyingAttack",em.IsReadyingAttack());
            if(em.IsAttacking() || em.IsReadyingAttack())
            {
                anim.SetFloat("moveX",em.TargetDirection(em.enemyTarget.transform.position).x);
                anim.SetFloat("moveY",em.TargetDirection(em.enemyTarget.transform.position).y);
            }
            else
            {
                anim.SetFloat("moveX",em.rb2d.linearVelocity.normalized.x);
                anim.SetFloat("moveY",em.rb2d.linearVelocity.normalized.y);
            }
            
            if(em.rb2d.linearVelocity.normalized.x > 0)
            {
                em.FlipSpriteRend(true);
            }
            else if(em.rb2d.linearVelocity.normalized.x < 0)
            {
                em.FlipSpriteRend(false);
            }
        }
    }
}
