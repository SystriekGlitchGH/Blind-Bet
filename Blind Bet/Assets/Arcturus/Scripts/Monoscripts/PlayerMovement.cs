using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = System.Random;

public class PlayerMovement : MonoBehaviour
{
    //Visuals
    [SerializeField] GameObject attackVisual;
    [SerializeField] GameObject whirlWindsVisual;
    [SerializeField] GameObject continuousBlade;
    [SerializeField] GameObject royalBomb;
    // ability sprites
    [SerializeField] GameObject chillingBurstVisual;
    [SerializeField] GameObject flashbangvisual;
    [SerializeField] GameObject spectralBullet;
    [SerializeField] GameObject parryObject;

    //Permanent components

    [Header("Components")]
    public Rigidbody2D rb2d;
    [SerializeField] SpriteRenderer spriteRend;
	public Transform anchorTransform;
    public Player playerStats;
    public Node currentNode;

	[Header("Movement stats")]
    public float acceleration; // how quickly you go to top speed
    public float friction; // controls air resistance
    private float directionX, directionY; // variables for direction when moving

    [Header("Attack stats")]
    private bool isDashing, canDash = true; // checks if you are currently dashing and are allowed to dash
    private bool isParrying, canParry = true; // checks if you are parrying and if you can parry
    private bool isLunging; // checks if you are currently lunging and are allowed to lunge
    public LayerMask attackLayer; // the layers that your attack can hit
    public LayerMask parryLayer; // the layers that your parry can hit
    private bool canAttack = true; // checks if you can attack
    private bool buttonHeld; // checks if the attack button is held for auto fire purposes
    private float attackAngle; // the angle of your attack
    public float iFrameTime;
    private bool hasIFrames;

    [Header("Ability stats")]
    private bool canUseAbility1 = true;

    [Header("Getting Attacked")]
    private bool hasKnockback;
    public float knockbackTime = 0.2f;

    // enum to make direction more readble
    private enum Direction
    {
        North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest
    }
    private Direction playerDirection; // the player's current direction
    Random rand = new Random();
    private void Awake()
    {
        playerStats = new Player(); // constructing player object
        playerStats.AddCard();
        playerStats.SetHandType(playerStats.activeHand,1);
        Debug.Log("active hand: "+playerStats.activeHand.cards[0].rank+playerStats.activeHand.cards[0].suit+
            ", "+playerStats.activeHand.cards[1].rank + playerStats.activeHand.cards[1].suit + 
            ", " + playerStats.activeHand.cards[2].rank + playerStats.activeHand.cards[2].suit + 
            ", " + playerStats.activeHand.cards[3].rank + playerStats.activeHand.cards[3].suit + 
            ", " + playerStats.activeHand.cards[4].rank + playerStats.activeHand.cards[4].suit);
        // Debug.Log("suit of active hand: "+playerStats.GetHandSuit(playerStats.activeHand));
        // Debug.Log("is suited: "+playerStats.IsSuited(playerStats.activeHand));
        Debug.Log("active hand type: "+playerStats.activeHand.type);
        playerStats.SetActiveAbility(playerStats.activeHand);
        Debug.Log("active ability: "+playerStats.activeAbility.name);
        
        playerStats.SetHandType(playerStats.passiveHand1,2);
        Debug.Log("passive hand 1: "+playerStats.passiveHand1.cards[0].rank+playerStats.passiveHand1.cards[0].suit+
            ", "+playerStats.passiveHand1.cards[1].rank + playerStats.passiveHand1.cards[1].suit + 
            ", " + playerStats.passiveHand1.cards[2].rank + playerStats.passiveHand1.cards[2].suit + 
            ", " + playerStats.passiveHand1.cards[3].rank + playerStats.passiveHand1.cards[3].suit + 
            ", " + playerStats.passiveHand1.cards[4].rank + playerStats.passiveHand1.cards[4].suit);
        // Debug.Log("suit of passive hand 1: "+playerStats.GetHandSuit(playerStats.passiveHand1));
        // Debug.Log("is suited: "+playerStats.IsSuited(playerStats.passiveHand1));
        Debug.Log("passive hand type 1: "+playerStats.passiveHand1.type);
        playerStats.SetPassiveAbility1(playerStats.passiveHand1);
        Debug.Log("passive ability 1: "+playerStats.passiveAbility1.name);
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
        Vector2 velocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), playerStats.baseSpeed*playerStats.GetSpeedMod());
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
            StartCoroutine(AttackTimer());
            if(playerStats.activeAbility.code == "a5")
                StartCoroutine(WhirlWindsTimer());
            if(playerStats.activeAbility.code == "a6")
                ActivateContinuousBlade();
            if (playerStats.activeAbility.code == "a7")
                StartCoroutine(EchoAttackTimer());
            if (playerStats.activeAbility.code == "a8")
                StartCoroutine(ExtraAttackTimer());
            if (playerStats.activeAbility.code == "a9")
            {
                StartCoroutine(ExtraAttackTimer());
                StartCoroutine(ExtraAttackTimer());
            }
            if (playerStats.activeAbility.code == "a10")
                ActivateRoyalBomb();
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
                StartCoroutine(RetaliationTimer());
            }
            if(hit && hit.rigidbody.TryGetComponent(out Bullet bullet))
            {
                Debug.Log("Parried");
                bullet.bulletType = "player";
                bullet.pm = this;
                bullet.em = null;
                bullet.rb2d.linearVelocity = -bullet.rb2d.linearVelocity;
                StartCoroutine(RetaliationTimer());
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
    public void PassiveAbility1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canUseAbility1)
        {
            if (playerStats.passiveAbility1.code == "b3d")
                StartCoroutine(ChillingBurstTimer());
            else if(playerStats.passiveAbility1.code == "b4d")
                StartCoroutine(FlashBangTimer());
            else if(playerStats.passiveAbility1.code == "b6d")
                StartCoroutine(SplinterCarbineTimer());
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
    }
    // getting hit
    public void GetHit(EnemyMovement attacker, float knockback)
    {
        if (!hasIFrames)
        {
            Debug.Log("got hit");
            StartCoroutine(GetHitTimer());
            rb2d.AddForce(attacker.TargetDirection(transform.position)*knockback,ForceMode2D.Impulse);
        }
    }
    // active abilities:
    private void ActivateContinuousBlade()
    {
        GameObject shot = Instantiate(continuousBlade, transform.position + (Vector3)DirectionToVector()* (playerStats.weapon.baseAttackSize.y * playerStats.GetAttackSizeMod()), anchorTransform.rotation);
        if (shot.TryGetComponent(out ContBlade ct))
        {
            ct.bulletType = "player";
            ct.pm = this;
            ct.direction = DirectionToVector();
            ct.rb2d.AddForce(ct.rb2d.transform.up * 750);
        }
    }
    private void ActivateRoyalBomb()
    {
        GameObject shot = Instantiate(royalBomb, transform.position + (Vector3)DirectionToVector(), anchorTransform.rotation);
        if (shot.TryGetComponent(out RoyalBomb rb))
        {
            rb.bulletType = "player";
            rb.pm = this;
            rb.direction = DirectionToVector();
            rb.rb2d.AddForce(rb.rb2d.transform.up * 1000);
        }
    }
    // passive abilities
    private void ActivateSplinterCarbine()
    {
        float extraRotation = -10/2;
        extraRotation += 10 / (6 - 1) * rand.Next(1,6);
        Vector3 rotation = anchorTransform.rotation.eulerAngles + new Vector3(0,0,extraRotation);
        GameObject shot = Instantiate(spectralBullet, transform.position + (Vector3)DirectionToVector(), Quaternion.Euler(rotation));
        if (shot.TryGetComponent(out SpecSplinterBullet sb))
        {
            sb.bulletType = "player";
            sb.pm = this;
            sb.direction = DirectionToVector();
            sb.rb2d.AddForce(sb.rb2d.transform.up * 1300);
        }
    }
    
    #endregion
    #region IENUMERATORS
    // active abilities
    private IEnumerator WhirlWindsTimer()
    {
        yield return new WaitForSeconds(0.2f);
        RaycastHit2D[] hits = MakeCircleCastAll("whirlwinds");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.weapon.baseKnockback/2*playerStats.GetAttackKnockbackMod(), playerStats.weapon.baseAttack * playerStats.GetAttackDamageMod() * 0.8f);
            }
        }
        // makes an attack visual sprite when using a melee attack
        GameObject attack = Instantiate(whirlWindsVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(6, 6) * playerStats.GetAttackSizeMod();

        yield return new WaitForSeconds(0.3f);
        Destroy(attack);
    }
    private IEnumerator EchoAttackTimer()
    {
        Vector3 pos = transform.position;
        float rot = attackAngle;
        yield return new WaitForSeconds(0.6f);
        RaycastHit2D[] hits = MakeBoxCastAll("echo", pos, rot);
        // makes the dash when attacking as spear
        if (playerStats.activeSuit == Card.Suit.spade)
            ActivateDash(1);
        // detecting and delivering hits
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
                enemy.GetHitAway(this, playerStats.weapon.baseKnockback*playerStats.GetAttackKnockbackMod(), playerStats.weapon.baseAttack * playerStats.GetAttackDamageMod());
            Debug.Log(playerStats.weapon.baseAttack * playerStats.GetAttackDamageMod());
        }
        // makes an attack visual sprite when using a melee attack
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * rot), Mathf.Cos(Mathf.Deg2Rad * rot));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y * playerStats.GetAttackSizeMod() / 2 + 1);
        GameObject attack = Instantiate(attackVisual, pos + (Vector3)position, Quaternion.Euler(0,0,rot));
        attack.transform.localScale = playerStats.weapon.baseAttackSize * playerStats.GetAttackSizeMod();

        yield return new WaitForSeconds(0.1f);
        Destroy(attack);
    }
    private IEnumerator ExtraAttackTimer()
    {
        yield return new WaitForSeconds(0.05f);
        RaycastHit2D[] hits = MakeBoxCastAll("attack");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
                enemy.GetHit(this, 0, playerStats.weapon.baseAttack * playerStats.GetAttackDamageMod());
            Debug.Log(playerStats.weapon.baseAttack*playerStats.GetAttackDamageMod());
        }
    }
    // passive abilities Diamond
    private IEnumerator ChillingBurstTimer()
    {
        canUseAbility1 = false;
        RaycastHit2D[] hits = MakeCircleCastAll("chillingburst");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod(), playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod() * 0.2f);
                if (!enemy.enemyStats.hasChill)
                {
                    enemy.enemyStats.AddEffect("chill", 5 * playerStats.GetAbilityEffectDurationMod());
                }
            }  
        }
        GameObject attack = Instantiate(chillingBurstVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(7, 7) * playerStats.GetAbilitySizeMod();
        yield return new WaitForSeconds(0.3f);
        Destroy(attack);
        yield return new WaitForSeconds(2*playerStats.GetAbilityCooldownMod()-0.3f);
        canUseAbility1 = true;
    }
    private IEnumerator FlashBangTimer()
    {
        canUseAbility1 = false;
        RaycastHit2D[] hits = MakeBoxCastAll("flashbang");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod(), playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod());
            }  
        }

        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        GameObject attack = Instantiate(flashbangvisual, transform.position + (Vector3)angleAsVector, anchorTransform.rotation, transform);
        attack.transform.localScale = new Vector2(1, 1) * playerStats.GetAbilitySizeMod();
        yield return new WaitForSeconds(0.2f);
        Destroy(attack);
        yield return new WaitForSeconds(2*playerStats.GetAbilityCooldownMod()-0.2f);
        canUseAbility1 = true;
    }
    private IEnumerator SplinterCarbineTimer()
    {
        canUseAbility1 = false;
        float timeBetweenBullets = 0.15f;
        for(int i = 0; i < 5; i++)
        {
            ActivateSplinterCarbine();
            yield return new WaitForSeconds(timeBetweenBullets);
        }
        yield return new WaitForSeconds(2*playerStats.GetAbilityCooldownMod());
        canUseAbility1 = true;
    }
    private IEnumerator AttackTimer()
    {
        canAttack = false;
        RaycastHit2D[] hits = MakeBoxCastAll("attack");
        // makes the dash when attacking as spear
        if (playerStats.activeSuit == Card.Suit.spade)
            ActivateDash(1);
        // detecting and delivering hits
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHit(this, playerStats.weapon.baseKnockback,playerStats.weapon.baseAttack*playerStats.GetAttackDamageMod());
                if (!enemy.enemyStats.hasPoison && playerStats.passiveAbility1.code == "n5d")
                {
                    enemy.enemyStats.AddEffect("poison", 6 * playerStats.GetAbilityEffectDurationMod());
                }
            }
            
        }
        // makes an attack visual sprite when using a melee attack
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()/2+1);
        if (isParrying) position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()+1);
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
        if (isParrying) attack.transform.localScale = playerStats.weapon.baseAttackSize*playerStats.GetAttackSizeMod()*2;
        else attack.transform.localScale = playerStats.weapon.baseAttackSize*playerStats.GetAttackSizeMod();
        
        yield return new WaitForSeconds(0.1f);
        Destroy(attack);
        // amount of time before you can attack again - time lost on visual animation on previous waitforseconds
        yield return new WaitForSeconds(TimeBetweenAttacks()-0.1f);
        canAttack = true;
    }
    private IEnumerator RetaliationTimer()
    {
        canAttack = false;
        RaycastHit2D[] hits = MakeBoxCastAll("retaliate");
        foreach (RaycastHit2D hit2 in hits)
        {
            if (hit2 && hit2.rigidbody.TryGetComponent(out EnemyMovement enemies))
            {
                enemies.GetHit(this, playerStats.weapon.baseKnockback*2, playerStats.weapon.baseAttack*playerStats.GetAttackDamageMod()*2);
            }
        }
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()/2+1);
        if (isParrying) position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()+1);
        GameObject attack = Instantiate(attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
        if (isParrying) attack.transform.localScale = playerStats.weapon.baseAttackSize*playerStats.GetAttackSizeMod()*2;
        else attack.transform.localScale = playerStats.weapon.baseAttackSize*playerStats.GetAttackSizeMod();
        
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
        hasIFrames = true;
        rb2d.AddForce(DirectionToVector()*playerStats.baseDashDistance, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        hasIFrames = false;
        yield return new WaitForSeconds(playerStats.baseDashCooldown);
        canDash = true;
    }
    // this is needed to make sure the dash cooldown doesn't break
    private IEnumerator LungeTimer()
    {
        isLunging = true;
        hasIFrames = true;
        rb2d.AddForce(DirectionToVector()*playerStats.baseDashDistance, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        isLunging = false;
        hasIFrames = false;
        yield return new WaitForSeconds(playerStats.baseDashCooldown);
    }
    // timer script for getting hit by anything
    private IEnumerator GetHitTimer()
    {
        hasKnockback = true;
        spriteRend.color = new Color32(150,0,0,255);
        hasIFrames = true;
        yield return new WaitForSeconds(knockbackTime);
        spriteRend.color = new Color32(255,255,255,255);
        hasKnockback = false;
        yield return new WaitForSeconds(iFrameTime - knockbackTime);
        hasIFrames = false;
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
        yield return new WaitForSeconds(playerStats.baseParryCooldown);
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
           Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()/2+1);
           return Physics2D.BoxCastAll(transform.position + (Vector3)position, playerStats.weapon.baseAttackSize*playerStats.GetAttackSizeMod(), attackAngle, Vector2.zero,0,attackLayer); 
        }
        else if(type == "retaliate")
        {
           Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()+1);
           return Physics2D.BoxCastAll(transform.position + (Vector3)position, playerStats.weapon.baseAttackSize*playerStats.GetAttackSizeMod()*2, attackAngle, Vector2.zero,0,attackLayer); 
        }
        else if(type == "echo")
        {
            Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y * playerStats.GetAttackSizeMod() / 2 + 1);
            return Physics2D.BoxCastAll(transform.position + (Vector3)position, playerStats.weapon.baseAttackSize * playerStats.GetAttackSizeMod(), attackAngle, Vector2.zero, 0, attackLayer);
        }
        else if(type == "flashbang")
        {
            Vector2 position = angleAsVector * (5 * playerStats.GetAbilitySizeMod() / 2 + 1);
            return Physics2D.BoxCastAll(transform.position + (Vector3)position, new Vector2(8,5) * playerStats.GetAbilitySizeMod(), attackAngle, Vector2.zero, 0, attackLayer);
        }
        else // dummy boxcast, does nothing
        {
            return Physics2D.BoxCastAll(transform.position, Vector3.zero, attackAngle, Vector2.zero);
        }
    }
    private RaycastHit2D[] MakeBoxCastAll(string type, Vector3 pos, float rot)
    {
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * rot), Mathf.Cos(Mathf.Deg2Rad * rot));
        if (type == "echo")
        {
            Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y * playerStats.GetAttackSizeMod() / 2 + 1);
            return Physics2D.BoxCastAll(pos + (Vector3)position, playerStats.weapon.baseAttackSize * playerStats.GetAttackSizeMod(), rot, Vector2.zero, 0, attackLayer);
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
           return Physics2D.BoxCast(transform.position + (Vector3)position, playerStats.weapon.baseParrySize, attackAngle, Vector2.zero,0,parryLayer); 
        }
        else // dummy boxcast, does nothing
        {
            return Physics2D.BoxCast(transform.position, Vector3.zero, attackAngle, Vector2.zero);
        }
    }
    
    private RaycastHit2D[] MakeCircleCastAll(string type)
    {
        if(type == "whirlwinds")
        {
            return Physics2D.CircleCastAll(transform.position, 3*playerStats.GetAttackSizeMod(), Vector2.zero, 0, attackLayer); 
        }
        else if (type == "chillingburst")
        {
            return Physics2D.CircleCastAll(transform.position, 3.5f, Vector2.zero, 0, attackLayer);
        }
        return null;
    }
    // equations for finding amount of time between singular attacks
    private float TimeBetweenAttacks()
    {
        return 1/(1+playerStats.weapon.baseAttackSpeed/100*playerStats.GetAttackSpeedMod());
    }

    #endregion
    // for debugging attack hitboxes
    //private void OnDrawGizmos()
    //{   
    //    if(playerStats != null)
    //    {
    //        Gizmos.matrix = anchorTransform.localToWorldMatrix;
    //        Gizmos.DrawWireCube(new Vector2(0,playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()/2+1), playerStats.weapon.baseAttackSize*playerStats.GetAttackSizeMod());
    //    }
    //}
}
