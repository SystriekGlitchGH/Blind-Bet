using UnityEngine;

public class SoldierAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] EnemyMovement em;
    private void Update()
    {
        if(em.enemyTarget != null)
        {
            if(em.rb2d.linearVelocity.magnitude > 0.5)
                anim.SetBool("isRunning",true);
            else
                anim.SetBool("isRunning",false);
            anim.SetFloat("moveX",em.rb2d.linearVelocity.normalized.x);
            anim.SetFloat("moveY",em.rb2d.linearVelocity.normalized.y);
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
