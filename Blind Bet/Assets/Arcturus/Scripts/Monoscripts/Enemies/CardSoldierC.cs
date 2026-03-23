using UnityEngine;

public class CardSoldierC : EnemyMovement
{
    public GameObject attackVisual;
    [SerializeField] Transform anchorTransform;
    public LayerMask boxLayer;
    public Vector2 attackSize;
    protected override void Update()
    {
        base.Update();
        if (enemyTarget != null)
        {
            // find the angle from a normalised vector2
            float angleRadians = Mathf.Atan2(TargetDirection(enemyTarget.transform.position).y, TargetDirection(enemyTarget.transform.position).x);
            //converts that angle to degrees, not radians
            float angleDegrees = angleRadians * Mathf.Rad2Deg;
            angleDegrees -= 90; // sets the rotation correctly by 90 degrees
            //anchorTransform.rotation = Quaternion.LookRotation(PlayerDirection(target.transform.position));
            anchorTransform.rotation = Quaternion.Euler(0, 0, angleDegrees);
        }
    }
}
