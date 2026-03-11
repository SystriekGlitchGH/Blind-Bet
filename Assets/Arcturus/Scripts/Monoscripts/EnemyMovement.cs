using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public PlayerMovement target;
    [Header("Components")]
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] SpriteRenderer spriteRend;
    [SerializeField] Enemy enemy;

    [Header("Movement Stats")]
    public float acceleration;
    public float topSpeed;
    public float friction;
    // variables to help with movement
    private float distance;
    private bool hasKnockback;
    public float knockbackTime;

    //other
    private float colliderPushForce = 8;

    private void Start()
    {
        rb2d.linearDamping = friction;
    }
    private void FixedUpdate()
    {
        if(target != null)
        {
            if (hasKnockback)
            {
                return;
            }
            distance = PlayerDistance(target.transform.position);
            if(distance > 2)
            {
                rb2d.linearDamping = 0;
                Vector2 newVelocity = PlayerDirection(target.transform.position)*acceleration;
                rb2d.AddForce(newVelocity);
                Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), topSpeed);
                rb2d.linearVelocity = velocity;
            }
            if(distance < 2)
            {
                rb2d.linearDamping = friction;
            }
            if(distance > 40)
            {
                target = null;
                rb2d.linearDamping = friction;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement pm = collision.GetComponent<PlayerMovement>();
            pm.rb2d.AddForce(PlayerDirection(pm.transform.position)*colliderPushForce);
        }
        else if (collision.CompareTag("Enemy"))
        {
            EnemyMovement em = collision.GetComponent<EnemyMovement>();
            em.rb2d.AddForce(PlayerDirection(em.transform.position)*colliderPushForce);
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    public void Hit(PlayerMovement attacker, float knockback)
    {
        StartCoroutine(GetHit());
        rb2d.AddForce(attacker.DirectionToVector()*knockback,ForceMode2D.Impulse);
    }

    private IEnumerator GetHit()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150,0,0,255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(90,215,0,255);
        hasKnockback = false;
    }

    //movement help methods
    private float PlayerDistance(Vector2 playerPos)
    {
        return Vector2.Distance((Vector2)transform.position, playerPos);
    }
    private Vector2 PlayerDirection(Vector2 playerPos)
    {
        return (playerPos - (Vector2)transform.position).normalized;
    }
}
