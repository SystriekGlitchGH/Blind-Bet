using UnityEngine;

public class SquidibleAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Area1Boss em;
    private void Update()
    {
        if(em.enemyTarget != null)
        {
            if(em.inRedAttack || em.inBlueAttack)
            {
                anim.SetFloat("moveX",em.TargetDirection(em.enemyTarget.transform.position).x);
                anim.SetFloat("moveY",em.TargetDirection(em.enemyTarget.transform.position).y);
                if(em.TargetDirection(em.enemyTarget.transform.position).x > 0)
                {
                    em.FlipSpriteRend(true);
                }
                else if(em.TargetDirection(em.enemyTarget.transform.position).x < 0)
                {
                    em.FlipSpriteRend(false);
                }
            }
            else if (!em.canWhiteAttack)
            {
                anim.SetFloat("moveX",0);
                anim.SetFloat("moveY",-1);
            }
            else
            {
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
}
