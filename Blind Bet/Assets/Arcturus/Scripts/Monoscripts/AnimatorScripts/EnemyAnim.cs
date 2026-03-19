using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] EnemyMovement em;
    private void Update()
    {
        if(em.target != null)
        {
            anim.SetFloat("moveX",em.PlayerDirection(em.target.transform.position).x);
            anim.SetFloat("moveY",em.PlayerDirection(em.target.transform.position).y);
            if(em.PlayerDirection(em.target.transform.position).x > 0)
            {
                em.FlipSpriteRend(true);
            }
            else if(em.PlayerDirection(em.target.transform.position).x < 0)
            {
                em.FlipSpriteRend(false);
            }
        }
        
    }
}
