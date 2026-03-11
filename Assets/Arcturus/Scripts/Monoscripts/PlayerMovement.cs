using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public class PlayerMovement : MonoBehaviour
{
    //Non-Permanant components
    [SerializeField] GameObject attackVisual;

    //Permanent components

    [Header("Components")]
    public Rigidbody2D rb2d;
    [SerializeField] SpriteRenderer spriteRend;
	[SerializeField] Transform anchorTransform;

	[Header("Movement stats")]
    // actual stats for how the player moves
    public float acceleration;
    public float topSpeed;
    public float friction;
    
    // variables for direction when moving
    private float directionX, directionY;

    [Header("Attack stats")]
    //variables for which weapon you are holding
    public string weaponName;
    public string weaponType;
    public float spearLungeForce;
    private bool isLunging = false;
    private bool inCombo = true;
    // tracks which weapon is currently held
    private int typeNum;
    private int weaponNum;
    public LayerMask boxLayer;
    public Weapon weapon;
    // tracks if you're attacking or not, and how you are attacking
    private bool canAttack = true;
    private bool buttonHeld;
    private float attackAngle;
    Random rand = new Random();
    private enum Direction
    {
        North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest
    }
    private Direction playerDirection;
    private void Awake()
    {
		weapon = new Weapon(weaponName, weaponType);
	}
    private void FixedUpdate()
    {
        if (isLunging)
        {
            rb2d.linearDamping = 0;
            return;
        }
        // adding acceleration to the directions
        Vector2 newVelocity = new Vector2(directionX * acceleration, directionY * acceleration);
        // adding the force to the rigidbody2d
        rb2d.AddForce(newVelocity);
        // maxing the velocity of the player to the topSpeed variable
        Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), topSpeed);
        // applying clamped velocity to rigidbody2d
        rb2d.linearVelocity = velocity;
        // when both direction inputs are 0, add linear damping to slow down player quickly
        if(directionX == 0 && directionY == 0) rb2d.linearDamping = friction;
        else rb2d.linearDamping = 0;
    }
    private void Update()
    {
        FindDirection();
        FindAngle();
        anchorTransform.eulerAngles = new Vector3(0,0,attackAngle);
    }
    public void Move(InputAction.CallbackContext ctx)
    {
        // direction x takes left and right, direction y takes up and down
        directionX = ctx.ReadValue<Vector2>().x;
        directionY = ctx.ReadValue<Vector2>().y;
    }
    public void Attack(InputAction.CallbackContext ctx)
    {
        if(ctx.ReadValue<float>() == 1 && canAttack)
        {
            buttonHeld = true;
            if(weaponType == "blade")
            {
                RaycastHit2D[] hits = MakeBoxCastAttack();
                // makes the dash when attacking as spear
                if (weaponName == "spear")
                {
                    StartCoroutine(SpearLunge());
                    rb2d.AddForce(DirectionToVector()*spearLungeForce, ForceMode2D.Impulse);
                }
                // starts the axe combo timer
                if(weaponName == "axe" && inCombo)
                {
                    StartCoroutine(Axe3Hit());
                }
                // detecting and delivering hits
                StartCoroutine(Attack());
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
                    {
                        Debug.Log("attack succesful");
                        enemy.Hit(this, weapon.baseKnockback);
                    }
                }
            }
        }
        if (ctx.ReadValue<float>() == 0)
        {
            buttonHeld = false;
        }
    }
    public void SwitchWeaponName(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            weaponNum++;
            if (typeNum == 0)
            {
                if(weaponNum == 1)
                {
                    weaponName = "axe";
                    weapon = new Weapon(weaponName, weaponType);
                }
                else if(weaponNum == 2)
                {
                    weaponName = "spear";
                    weapon = new Weapon(weaponName, weaponType);
                }
                else if(weaponNum == 3)
                {
                    weaponNum = 0;
                    weaponName = "sword";
                    weapon = new Weapon(weaponName, weaponType);
                }
            }
        }
    }
    private void FindAngle()
    {
        if(playerDirection == Direction.North) attackAngle = 0;
        if(playerDirection == Direction.South) attackAngle = 180;
        if(playerDirection == Direction.East) attackAngle = -90;
        if(playerDirection == Direction.West) attackAngle = 90;
        if(playerDirection == Direction.NorthEast) attackAngle = -45;
        if(playerDirection == Direction.NorthWest) attackAngle = 45;
        if(playerDirection == Direction.SouthEast) attackAngle = -135;
        if(playerDirection == Direction.SouthWest) attackAngle = 135;
    }
    private void FindDirection()
    {
        //north
        if(directionX == 0 && directionY > 0) playerDirection = Direction.North;
        //south
        if(directionX == 0 && directionY < 0) playerDirection = Direction.South;
        //east
        if(directionX > 0 && directionY == 0) playerDirection = Direction.East;
        //west
        if(directionX < 0 && directionY == 0) playerDirection = Direction.West;
        //northeast
        if(directionX > 0 && directionY > 0) playerDirection = Direction.NorthEast;
        //northwest
        if(directionX < 0 && directionY > 0) playerDirection = Direction.NorthWest;
        //southeast
        if(directionX > 0 && directionY < 0) playerDirection = Direction.SouthEast;
        //southwest
        if(directionX < 0 && directionY < 0) playerDirection = Direction.SouthWest;
    }
    public Vector2 DirectionToVector()
    {
        if(playerDirection == Direction.North) return new Vector2(0,1);
        if(playerDirection == Direction.South) return new Vector2(0,-1);
        if(playerDirection == Direction.East) return new Vector2(1,0);
        if(playerDirection == Direction.West) return new Vector2(-1,0);
        if(playerDirection == Direction.NorthEast) return new Vector2(0.7071f,0.7071f);
        if(playerDirection == Direction.NorthWest) return new Vector2(-0.7071f,0.7071f);
        if(playerDirection == Direction.SouthEast) return new Vector2(0.7071f,-0.7071f);
        if(playerDirection == Direction.SouthWest) return new Vector2(-0.7071f,-0.7071f);
        return new Vector2(0,0);
    }
    private RaycastHit2D[] MakeBoxCastAttack()
    {
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * weapon.baseAttackDistance;
		return Physics2D.BoxCastAll(transform.position + (Vector3)position, weapon.baseAttackSize, attackAngle, Vector2.zero,0,boxLayer);
    }
    private IEnumerator Attack()
    {
        canAttack = false;
        // all this code is purely for visual during presentation, will be replaced with animator sprites from here
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * weapon.baseAttackDistance;
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, anchorTransform);
        attack.transform.localScale = weapon.baseAttackSize;
        // to here
        
        yield return new WaitForSeconds(0.1f);
        Destroy(attack);
        yield return new WaitForSeconds(2/weapon.baseAttackSpeed-0.1f);
        canAttack = true;
    }
    private IEnumerator SpearLunge()
    {
        isLunging = true;
        yield return new WaitForSeconds(0.2f);
        isLunging = false;
    }
    private IEnumerator Axe3Hit()
    {
        weapon.baseAttackSpeed = 10;
        weapon.baseKnockback = 6;
        yield return new WaitForSeconds(0.6f);
        inCombo = false;
        weapon.baseAttackSpeed = 2;
        weapon.baseKnockback = 20;
        yield return new WaitForSeconds(4);
        inCombo = true;
    }
    private void OnDrawGizmos()
    {   
        if(weapon != null)
        {
            Gizmos.matrix = anchorTransform.localToWorldMatrix;
            Gizmos.DrawWireCube(new Vector2(0,weapon.baseAttackDistance), weapon.baseAttackSize);
        }
    }
}
