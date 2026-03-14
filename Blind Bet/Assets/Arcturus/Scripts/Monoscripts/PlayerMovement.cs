using System;
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
    public Player playerStats;

	[Header("Movement stats")]
    public float acceleration; // how quickly you go to top speed
    public float friction; // controls air resistance
    private float directionX, directionY; // variables for direction when moving

    [Header("Attack stats")]
    private bool isDashing, canDash = true; // checks if you are currently dashing and are allowed to dash
    private bool isLunging; // checks if you are currently lunging and are allowed to lunge
    private bool inCombo = true; // checks if you are currently in an axe combo
    public LayerMask boxLayer; // the layers that your attack can hit
    private bool canAttack = true; // checks if you can attack
    private bool buttonHeld; // checks if the attack button is held for auto fire purposes
    private float attackAngle; // the angle of your attack
    // enum to make direction more readble
    private enum Direction
    {
        North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest
    }
    private Direction playerDirection; // the player's current direction
    private void Awake()
    {
        playerStats = new Player(); // constructing player object
	}
    private void FixedUpdate()
    {
        // if you are currently lunging, your lineardamping should be 0 and regular movement shouldn't apply
        if (isDashing || isLunging)
        {
            rb2d.linearDamping = 0;
            return;
        }
        // adding acceleration to the directions
        Vector2 newVelocity = new Vector2(directionX * acceleration, directionY * acceleration);
        // adding the force to the rigidbody2d
        rb2d.AddForce(newVelocity);
        // maxing the velocity of the player to the playerStats.baseSpeed variable
        Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), playerStats.baseSpeed);
        // applying clamped velocity to rigidbody2d
        rb2d.linearVelocity = velocity;
        // when both direction inputs are 0, add linear damping to slow down player quickly
        if(directionX == 0 && directionY == 0) rb2d.linearDamping = friction;
        else rb2d.linearDamping = 0;
    }
    private void Update()
    {
        FindDirection(); // gets the direction from the vector
        FindAngle(); // gets the angle from the direction
        anchorTransform.eulerAngles = new Vector3(0,0,attackAngle); // uses angle to change the achor transform
    }
    //TEMP CODE, DELETE WHEN CARD PICKING IS MADE
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Diamond"))
        {
            playerStats.suit = Player.Suit.diamond;
        }
        if (collision.CompareTag("Club"))
        {
            playerStats.suit = Player.Suit.club;
        }
        if (collision.CompareTag("Spade"))
        {
            playerStats.suit = Player.Suit.spade;
        }
        playerStats.weapon = new Weapon(playerStats.suit);
    }
    // gives directions from inputs
    public void Move(InputAction.CallbackContext ctx)
    {
        // direction x takes left and right, direction y takes up and down
        directionX = ctx.ReadValue<Vector2>().x;
        directionY = ctx.ReadValue<Vector2>().y;
    }
    // main input attacking script
    public void Attack(InputAction.CallbackContext ctx)
    {
        if(ctx.ReadValue<float>() == 1 && canAttack && playerStats.suit != Player.Suit.blank)
        {
            Debug.Log(playerStats.suit);
            buttonHeld = true;
            RaycastHit2D[] hits = MakeBoxCastAttack();
            // starts the axe combo timer
            if(playerStats.suit == Player.Suit.club && inCombo)
            {
                StartCoroutine(Axe3Hit());
            }
            // makes the dash when attacking as spear
            if (playerStats.suit == Player.Suit.spade)
            {
                ActivateDash(1);
            }
            // detecting and delivering hits
            StartCoroutine(Attack());
            foreach (RaycastHit2D hit in hits)
            {
                if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
                {
                    Debug.Log("attack succesful");
                    enemy.Hit(this, playerStats.weapon.baseKnockback);
                }
            }
        }
        if (ctx.ReadValue<float>() == 0)
        {
            buttonHeld = false;
        }
    }
    // input for dashing
    public void Dash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canDash)
        {
            ActivateDash(0);
        }
    }
    // makes a boxcastall that is the size of the weapon
    private RaycastHit2D[] MakeBoxCastAttack()
    {
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y/2+1);
		return Physics2D.BoxCastAll(transform.position + (Vector3)position, playerStats.weapon.baseAttackSize, attackAngle, Vector2.zero,0,boxLayer);
    }
    // attacking script for melee attacks
    private void ActivateDash(int type)
    {
        if(type == 0)
            StartCoroutine(Dash());
        if(type == 1)
            StartCoroutine(Lunge());
        rb2d.AddForce(DirectionToVector()*playerStats.dashDistance, ForceMode2D.Impulse);
    }
    private IEnumerator Attack()
    {
        canAttack = false;
        // makes an attack visual sprite when using a melee attack
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y/2+1);
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, anchorTransform);
        attack.transform.localScale = playerStats.weapon.baseAttackSize;
        
        yield return new WaitForSeconds(0.1f);
        Destroy(attack);
        // amount of time before you can attack again - time lost on visual animation on previous waitforseconds
        yield return new WaitForSeconds(TimeBetweenAttacks()-0.1f);
        canAttack = true;
    }
    // timer script for allowing the dash
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        yield return new WaitForSeconds(TimeBetweenDashes());
        canDash = true;
    }
    // this is needed to make sure the dash cooldown doesn't break
    private IEnumerator Lunge()
    {
        isLunging = true;
        yield return new WaitForSeconds(0.2f);
        isLunging = false;
        yield return new WaitForSeconds(TimeBetweenDashes());
    }
    // timer script for the axe combo attack
    private IEnumerator Axe3Hit()
    {
        playerStats.weapon.baseAttackSpeed = 400;
        playerStats.weapon.baseKnockback = 6;
        yield return new WaitForSeconds(0.6f);
        inCombo = false;
        playerStats.weapon.baseAttackSpeed = 2;
        playerStats.weapon.baseKnockback = 20;
        yield return new WaitForSeconds(4);
        inCombo = true;
    }
    // assigns attack angle from the corresponding direction
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
    // assigns the playerdirection by taking the input directions
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
    //takes current enum direction and returns the vector that is assigned to it
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
    // equations for finding amount of time between singular attacks
    private float TimeBetweenAttacks()
    {
        return 1/(1+(playerStats.AttackSpeed-100 + playerStats.weapon.baseAttackSpeed)/100);
    }
    private float TimeBetweenDashes()
    {
        return 0.5f/(1+(playerStats.dashCooldown-100)/100);
    }
    // for debugging attack hitboxes
    private void OnDrawGizmos()
    {   
        if(playerStats != null)
        {
            Gizmos.matrix = anchorTransform.localToWorldMatrix;
            Gizmos.DrawWireCube(new Vector2(0,playerStats.weapon.baseAttackSize.y/2+1), playerStats.weapon.baseAttackSize);
        }
    }
}
