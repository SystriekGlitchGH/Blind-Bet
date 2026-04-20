using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement pm;
    void Update()
    {
        anim.SetFloat("moveX", pm.rb2d.linearVelocity.normalized.x);
        anim.SetFloat("moveY", pm.rb2d.linearVelocity.normalized.y);
    }
}
