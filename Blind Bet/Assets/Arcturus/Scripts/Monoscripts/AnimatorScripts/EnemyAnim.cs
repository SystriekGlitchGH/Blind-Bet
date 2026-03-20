using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] EnemyMovement em;
    private void Update()
    {
        if(em.enemyTarget != null)
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
        
    }
}
