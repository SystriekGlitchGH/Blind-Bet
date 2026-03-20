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
    public GameObject attackVisual;
    [SerializeField] GameObject parryObject;

    //Permanent components

    [Header("Components")]
    public Rigidbody2D rb2d;
    [SerializeField] SpriteRenderer spriteRend;
	public Transform anchorTransform;
    public Player playerStats;

	[Header("Movement stats")]
    public float acceleration; // how quickly you go to top speed
    public float friction; // controls air resistance
    private float directionX, directionY; // variables for direction when moving

    [Header("Attack stats")]
    private bool isDashing, canDash = true; // checks if you are currently dashing and are allowed to dash
    private bool isParrying, canParry = true; // checks if you are parrying and if you can parry
    private bool isLunging; // checks if you are currently lunging and are allowed to lunge
    private bool inCombo = true; // checks if you are currently in an axe combo
    public LayerMask boxLayer; // the layers that your attack can hit
    private bool canAttack = true; // checks if you can attack
    private bool buttonHeld; // checks if the attack button is held for auto fire purposes
    private float attackAngle; // the angle of your attack

    [Header("Getting Attacked")]
    private bool hasKnockback;
    public float knockbackTime = 0.2f;

    // enum to make direction more readble
    private enum Direction
    {
        North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest
    }
    private Direction playerDirection; // the player's current direction
    private void Awake()
    {
        playerStats = new Player(); // constructing player object
        playerStats.AddCard();
        Debug.Log("cards: "+playerStats.activeHand.cards[0].rank+playerStats.activeHand.cards[0].suit+
            ", "+playerStats.activeHand.cards[1].rank + playerStats.activeHand.cards[1].suit + 
            ", " + playerStats.activeHand.cards[2].rank + playerStats.activeHand.cards[2].suit + 
            ", " + playerStats.activeHand.cards[3].rank + playerStats.activeHand.cards[3].suit + 
            ", " + playerStats.activeHand.cards[4].rank + playerStats.activeHand.cards[4].suit);
        Debug.Log("suit of hand: "+playerStats.GetHandSuit(playerStats.activeHand));
        Debug.Log("is suited: "+playerStats.IsSuited(playerStats.activeHand));
        Debug.Log("hand type: "+playerStats.activeHand.type);
    }
    private void FixedUpdate()
    {
        // if you are currently lunging, your lineardamping should be 0 and regular movement shouldn't apply
        if (isDashing || isLunging || hasKnockback)
        {
            rb2d.linearDamping = 0;
            return;
        }
        if (isParrying)
        {
            rb2d.linearVelocity = Vector2.zero;
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
            playerStats.activeSuit = Card.Suit.diamond;
        }
        if (collision.CompareTag("Club"))
        {
            playerStats.activeSuit = Card.Suit.club;
        }
        if (collision.CompareTag("Spade"))
        {
            playerStats.activeSuit = Card.Suit.spade;
        }
        playerStats.weapon = new Weapon(playerStats.activeSuit);
    }
    #region INPUTS
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
        if(ctx.ReadValue<float>() == 1 && canAttack && playerStats.activeSuit != Card.Suit.blank)
        {
            buttonHeld = true;
            RaycastHit2D[] hits = MakeBoxCastAll("attack");
            // starts the axe combo timer
            if(playerStats.activeSuit == Card.Suit.club && inCombo)
            {
                StartCoroutine(Axe3HitTimer());
            }
            // makes the dash when attacking as spear
            if (playerStats.activeSuit == Card.Suit.spade)
            {
                ActivateDash(1);
            }
            // detecting and delivering hits
            StartCoroutine(AttackTimer());
            foreach (RaycastHit2D hit in hits)
            {
                if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
                {
                    enemy.GetHit(this, playerStats.weapon.baseKnockback,playerStats.weapon.baseAttack);
                }
            }
        }
        if (ctx.ReadValue<float>() == 0)
        {
            buttonHeld = false;
        }
    }
    // input for parrying
    public void Parry(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canParry)
        {
            RaycastHit2D hit = MakeBoxCast("parry");
            StartCoroutine(ParryTimer());
            if(hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy) && enemy.IsAttacking())
            {
                enemy.setVelocity(Vector2.zero);
                RaycastHit2D[] hits = MakeBoxCastAll("retaliate");
                StartCoroutine(AttackTimer());
                foreach (RaycastHit2D hit2 in hits)
                {
                    if (hit2 && hit2.rigidbody.TryGetComponent(out EnemyMovement enemies))
                    {
                        enemies.GetHit(this, playerStats.weapon.baseKnockback*2, playerStats.weapon.baseAttack*2);
                    }
                }
            }
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
    #endregion
    #region ACTIVATION METHODS
    private void ActivateDash(int type)
    {
        if(type == 0)
            StartCoroutine(DashTimer());
        if(type == 1)
            StartCoroutine(LungeTimer());
        rb2d.AddForce(DirectionToVector()*playerStats.dashDistance, ForceMode2D.Impulse);
    }
    // getting hit
    public void GetHit(EnemyMovement attacker, float knockback)
    {
        StartCoroutine(GetHitTimer());
        rb2d.AddForce(attacker.TargetDirection(transform.position)*knockback,ForceMode2D.Impulse);
    }
    #endregion
    #region IENUMERATORS
    private IEnumerator AttackTimer()
    {
        canAttack = false;
        // makes an attack visual sprite when using a melee attack
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y/2+1);
        if (isParrying) position = angleAsVector * (playerStats.weapon.baseAttackSize.y+1);
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
        if (isParrying) attack.transform.localScale = playerStats.weapon.baseAttackSize*2;
        else attack.transform.localScale = playerStats.weapon.baseAttackSize;
        
        yield return new WaitForSeconds(0.1f);
        Destroy(attack);
        // amount of time before you can attack again - time lost on visual animation on previous waitforseconds
        yield return new WaitForSeconds(TimeBetweenAttacks()-0.1f);
        canAttack = true;
    }
    // timer script for allowing the dash
    private IEnumerator DashTimer()
    {
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        yield return new WaitForSeconds(playerStats.dashCooldown);
        canDash = true;
    }
    // this is needed to make sure the dash cooldown doesn't break
    private IEnumerator LungeTimer()
    {
        isLunging = true;
        yield return new WaitForSeconds(0.2f);
        isLunging = false;
        yield return new WaitForSeconds(playerStats.dashCooldown);
    }
    // timer script for the axe combo attack
    private IEnumerator Axe3HitTimer()
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
    // timer script for getting hit by anything
    private IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150,0,0,255);
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(255,255,255,255);
        hasKnockback = false;
    }
    private IEnumerator ParryTimer()
    {
        canParry = false;
        isParrying = true;
        // makes the parry visual when parrying
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseParrySize.y/2+1);
        GameObject parry = Instantiate(parryObject, transform.position + (Vector3)position, anchorTransform.rotation, anchorTransform);
        parry.transform.localScale = playerStats.weapon.baseParrySize;
        // to here
        yield return new WaitForSeconds(0.1f);
        Destroy(parry);
        yield return new WaitForSeconds(playerStats.baseParryTime-0.1f);
        isParrying = false;
        yield return new WaitForSeconds(playerStats.parryCooldown);
        canParry = true;
    }
    #endregion
    #region HELP METHODS
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
    // makes a boxcastAll that is the size of the weapon
    private RaycastHit2D[] MakeBoxCastAll(string type)
    {
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        if(type == "attack")
        {
           Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y/2+1);
           return Physics2D.BoxCastAll(transform.position + (Vector3)position, playerStats.weapon.baseAttackSize, attackAngle, Vector2.zero,0,boxLayer); 
        }
        if(type == "retaliate")
        {
           Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y+1);
           return Physics2D.BoxCastAll(transform.position + (Vector3)position, playerStats.weapon.baseAttackSize*2, attackAngle, Vector2.zero,0,boxLayer); 
        }
        else // dummy boxcast, does nothing
        {
            return Physics2D.BoxCastAll(transform.position, Vector3.zero, attackAngle, Vector2.zero);
        }
    }
    private RaycastHit2D MakeBoxCast(string type)
    {
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        if(type == "parry")
        {
           Vector2 position = angleAsVector * (playerStats.weapon.baseParrySize.y/2+1);
           return Physics2D.BoxCast(transform.position + (Vector3)position, playerStats.weapon.baseParrySize, attackAngle, Vector2.zero,0,boxLayer); 
        }
        else // dummy boxcast, does nothing
        {
            return Physics2D.BoxCast(transform.position, Vector3.zero, attackAngle, Vector2.zero);
        }
    }
    // equations for finding amount of time between singular attacks
    private float TimeBetweenAttacks()
    {
        return 1/(1+(playerStats.AttackSpeed-100 + playerStats.weapon.baseAttackSpeed)/100);
    }

    #endregion
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
