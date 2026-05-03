using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement pm;
    void Update()
    {
        if(pm.GetDirectionX() != 0 || pm.GetDirectionY() != 0)
            anim.SetBool("isRunning", true);
        else
            anim.SetBool("isRunning", false);
        anim.SetFloat("moveX", pm.DirectionToVector().x);
        anim.SetFloat("moveY", pm.DirectionToVector().y);
    }
}
