using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement pm;
    void Update()
    {
        anim.SetFloat("moveX", pm.DirectionToVector().x);
        anim.SetFloat("moveY", pm.DirectionToVector().y);
    }
}
