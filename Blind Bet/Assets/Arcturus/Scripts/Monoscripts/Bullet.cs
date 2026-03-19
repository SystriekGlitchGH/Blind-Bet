using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public PlayerMovement pm;
    public EnemyMovement em;
    public string bulletType;

    public Vector2 direction;
    private int enemiesHit;
    private float elapsedTime;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > 3)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(bulletType == "enemy")
        {
            if (collision.CompareTag("Player"))
            {
                PlayerMovement player = collision.GetComponent<PlayerMovement>();
                player.GetHit(em, em.baseKnockback);
                enemiesHit++;
                if(enemiesHit == 1)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if(bulletType == "player")
        {
            
        }
    }
}
