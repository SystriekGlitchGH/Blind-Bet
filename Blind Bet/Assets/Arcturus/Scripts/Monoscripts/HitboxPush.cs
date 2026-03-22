using UnityEngine;

public class HitboxPush : MonoBehaviour
{
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.rb2d.AddForce((pm.transform.position - transform.position).normalized*8);
        }
        else if (collision.CompareTag("Enemy"))
        {
            EnemyMovement em = collision.GetComponent<EnemyMovement>();
            em.rb2d.AddForce((em.transform.position - transform.position).normalized*8);
        }
    }
}
