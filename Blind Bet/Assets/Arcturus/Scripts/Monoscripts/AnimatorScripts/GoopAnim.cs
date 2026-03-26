using UnityEngine;

public class GoopAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] EnemyMovement em;
    private void Update()
    {
        if(em.enemyTarget != null)
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
