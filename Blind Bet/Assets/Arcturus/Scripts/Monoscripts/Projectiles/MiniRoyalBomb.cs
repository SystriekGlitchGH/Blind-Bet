using System.Collections;
using UnityEngine;

public class MiniRoyalBomb : MonoBehaviour
{
    [SerializeField] GameObject explosionVisual;
    [SerializeField] SpriteRenderer spriteRend;
    public Transform followTransform;
    public PlayerMovement pm;
    private float elapsedTime;
    private bool exploded;
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > 2 && !exploded)
        {
            StartCoroutine(ExplodeTimer());
        }
        if(followTransform != null)
        {
            transform.position = followTransform.position;
        }
    }
    private IEnumerator ExplodeTimer()
    {
        exploded = true;
        spriteRend.enabled = false;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.collider.TryGetComponent(out EnemyMovement enemy))
                enemy.GetHitAway(pm, 0, pm.playerStats.baseAbilityDamage*pm.playerStats.GetAbilityDamageMod()*2.5f);
        }
        GameObject attack = Instantiate(explosionVisual, transform.position, Quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(8f, 8f);
        yield return new WaitForSeconds(0.2f);
        Destroy(attack);
        Destroy(gameObject);
    }
}
