using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = System.Random;

public class PlayerMovement : MonoBehaviour
{
    //Permanent components

    [Header("Components")]
    public Rigidbody2D rb2d;
    [SerializeField] SpriteRenderer spriteRend;
	public Transform anchorTransform;
    public Player playerStats;
    public PrefabLibrary prefabLib;
    public Node currentNode;

	[Header("Movement stats")]
    public float acceleration; // how quickly you go to top speed
    public float friction; // controls air resistance
    private float directionX, directionY; // variables for direction when moving

    [Header("Attack stats")]
    private bool isDashing, canDash = true; // checks if you are currently dashing and are allowed to dash
    private bool hyperDash;

    private bool isParrying, canParry = true; // checks if you are parrying and if you can parry
    private bool isLunging; // checks if you are currently lunging and are allowed to lunge
    public LayerMask attackLayer; // the layers that your attack can hit
    public LayerMask parryLayer; // the layers that your parry can hit
    private bool canAttack = true; // checks if you can attack
    private float attackAngle; // the angle of your attack
    public float iFrameTime;
    private bool hasIFrames;

    // for holding abilities
    private bool buttonHeld; // checks if the attack button is held for hold abiliies
    private float buttonHeldTime;
    private bool indicatorShown;

    [Header("Feedback Colors")]
    public Color32 baseColor;
    public Color32 currentColor;

    [Header("Ability stats")]
    private bool canUseAbility1 = true;
    private bool isCharging;

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
        if (isCharging)
        {
            rb2d.linearVelocity = rb2d.linearVelocity * 5;
            Vector2 newChargeVelocity = new Vector2(directionX * acceleration, directionY * acceleration/2);
            rb2d.AddForce(newChargeVelocity);
            Vector2 chargeVelocity = Vector2.ClampMagnitude(new(rb2d.linearVelocity.x, rb2d.linearVelocity.y), playerStats.baseSpeed * playerStats.GetSpeedMod()*1.5f);
            rb2d.linearVelocity = chargeVelocity;
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
        if (buttonHeld)
        {
            buttonHeldTime += Time.deltaTime;
            if(buttonHeldTime >= 3 && !indicatorShown)
            {
                if(playerStats.passiveAbility1.code == "n8d" || playerStats.passiveAbility1.code == "n9d")
                StartCoroutine(DiamondIndicatorTimer());
                indicatorShown = true;
            }
        }
        playerStats.CheckEffects();
        if (playerStats.effectManager.effects.Count != 0)
        {
            for (int i = 0; i < playerStats.effectManager.effects.Count; i++)
            {
                playerStats.effectManager.effects[i].elapsedTime += Time.deltaTime;
                if (playerStats.effectManager.effects[i].elapsedTime >= playerStats.effectManager.effects[i].duration)
                {
                    playerStats.effectManager.effects.Remove(playerStats.effectManager.effects[i]);
                    playerStats.CheckEffects();
                    CheckCurrentColor();
                    spriteRend.color = currentColor;
                }
            }
        }
        CheckCurrentColor();
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
        if (collision.CompareTag("Enemy"))
        {
            EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
            if (isDashing)
            {
                enemy.GetHit(this,0,playerStats.baseDashDamage*playerStats.GetDashDamageMod());
            }
            if (isCharging)
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback / 2, playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod());
            }
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
            indicatorShown = false;
            if(buttonHeldTime >= 3 && playerStats.passiveAbility1.code == "n8d")
            {
                buttonHeldTime = 0;
                StartCoroutine(ShockingWheelTimer());
            }
            if(buttonHeldTime >= 3 && playerStats.passiveAbility1.code == "n9d")
            {
                buttonHeldTime = 0;
                StartCoroutine(FreezingWheelTimer());
            }
            if(buttonHeldTime >= 3 && playerStats.passiveAbility1.code == "n8h")
            {
                buttonHeldTime = 0;
                StartCoroutine(ShieldingWardTimer());
            }
            if(buttonHeldTime >= 3 && playerStats.passiveAbility1.code == "n9h")
            {
                buttonHeldTime = 0;
                StartCoroutine(ShieldingWardTimer());
                StartCoroutine(HyperDashTimer());
            }
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
            // diamonds
            if (playerStats.passiveAbility1.code == "b3d")
                StartCoroutine(ChillingBurstTimer());
            else if (playerStats.passiveAbility1.code == "b4d")
                StartCoroutine(FlashBangTimer());
            else if (playerStats.passiveAbility1.code == "b6d")
                StartCoroutine(SplinterCarbineTimer());
            else if (playerStats.passiveAbility1.code == "b7d")
                StartCoroutine(PiercingDuetTimer());
            else if (playerStats.passiveAbility1.code == "b10d")
                StartCoroutine(CombustionCarbineTimer());
            // hearts
            else if (playerStats.passiveAbility1.code == "b3h")
                StartCoroutine(HyperDashTimer());
            else if (playerStats.passiveAbility1.code == "b4h")
                ActivateHealingSigil();
            else if (playerStats.passiveAbility1.code == "b6h")
                ActivateWitheringPistol();
            else if (playerStats.passiveAbility1.code == "b7h")
                StartCoroutine(AccultSacrificeTimer());
            else if (playerStats.passiveAbility1.code == "b10h")
                ActivateDrainingMortar();
            // clubs
            else if (playerStats.passiveAbility1.code == "b3c")
                StartCoroutine(UnyieldingChargeTimer());
            else if (playerStats.passiveAbility1.code == "b4c")
                StartCoroutine(EarthBreakTimer());

            StartCoroutine(Ability1Timer());
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
    public void GetHit(EnemyMovement attacker, float knockback, float damage)
    {
        if (!hasIFrames)
        {
            Debug.Log("got hit for: "+ damage * playerStats.GetDamageMod());
            StartCoroutine(GetHitTimer());
            playerStats.TakeDamage(damage * playerStats.GetDamageMod());
            rb2d.AddForce(attacker.TargetDirection(transform.position)*knockback,ForceMode2D.Impulse);
        }
    }
    public void GetHealed(float healAmount)
    {
        StartCoroutine(GetHealedTimer());
        playerStats.Heal(healAmount);
    }
    // active abilities:
    #region ABILITIES
    private void ActivateContinuousBlade()
    {
        GameObject shot = Instantiate(prefabLib.continuousBlade, transform.position + (Vector3)DirectionToVector()* (playerStats.weapon.baseAttackSize.y * playerStats.GetAttackSizeMod()), anchorTransform.rotation);
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
        GameObject shot = Instantiate(prefabLib.royalBomb, transform.position + (Vector3)DirectionToVector(), anchorTransform.rotation);
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
        GameObject shot = Instantiate(prefabLib.spectralBullet, transform.position + (Vector3)DirectionToVector(), Quaternion.Euler(rotation));
        if (shot.TryGetComponent(out SpecSplinterBullet sb))
        {
            sb.bulletType = "player";
            sb.pm = this;
            sb.direction = DirectionToVector();
            sb.rb2d.AddForce(sb.rb2d.transform.up * 1300);
        }
    }
    private void ActivateCombustionCarbine()
    {
        float extraRotation = -10/2;
        extraRotation += 10 / (6 - 1) * rand.Next(1,6);
        Vector3 rotation = anchorTransform.rotation.eulerAngles + new Vector3(0,0,extraRotation);
        GameObject shot = Instantiate(prefabLib.royalBombAbility, transform.position + (Vector3)DirectionToVector(), Quaternion.Euler(rotation));
        if (shot.TryGetComponent(out RoyalBombAbility rb))
        {
            rb.bulletType = "player";
            rb.pm = this;
            rb.direction = DirectionToVector();
            rb.rb2d.AddForce(rb.rb2d.transform.up * 1000);
        }
    }
    private void ActivatePiercingDuet()
    {
        GameObject shot = Instantiate(prefabLib.soundWave, transform.position + (Vector3)DirectionToVector(), anchorTransform.rotation);
        if (shot.TryGetComponent(out SoundWave sw))
        {
            sw.bulletType = "player";
            sw.pm = this;
            sw.direction = DirectionToVector();
            sw.rb2d.AddForce(sw.rb2d.transform.up * 1000);
        }
    }
    private void ActivateWitheringPistol()
    {
        float extraRotation = -15 / 2;
        for (int i = 0; i < 2; i++)
        {
            Vector3 rotation = anchorTransform.rotation.eulerAngles + new Vector3(0, 0, extraRotation);
            GameObject shot = Instantiate(prefabLib.witheringBullet, transform.position + (Vector3)DirectionToVector(), Quaternion.Euler(rotation));
            if (shot.TryGetComponent(out CharmingBullet cb))
            {
                cb.bulletType = "player";
                cb.pm = this;
                cb.direction = DirectionToVector();
                cb.rb2d.AddForce(cb.rb2d.transform.up * 1300);
            }
            extraRotation += 15 / (2 - 1);
        }
        
    }
    private void ActivateDrainingMortar()
    {
        float extraRotation = -15 / 2;
        for (int i = 0; i < 2; i++)
        {
            Vector3 rotation = anchorTransform.rotation.eulerAngles + new Vector3(0, 0, extraRotation);
            GameObject shot = Instantiate(prefabLib.witheringBomb, transform.position + (Vector3)DirectionToVector(), Quaternion.Euler(rotation));
            if (shot.TryGetComponent(out WitheringBomb wb))
            {
                wb.bulletType = "player";
                wb.pm = this;
                wb.direction = DirectionToVector();
                wb.rb2d.AddForce(wb.rb2d.transform.up * 1300);
            }
            extraRotation += 15 / (2 - 1);
        }

    }
    private void ActivateHealingSigil()
    {
        GetHealed(20);
    }
    #endregion
    #endregion
    #region IENUMERATORS
    // active abilities
    #region ACTIVE AND DIAMOND
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
        GameObject attack = Instantiate(prefabLib.whirlWindsVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
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
        GameObject attack = Instantiate(prefabLib.attackVisual, pos + (Vector3)position, Quaternion.Euler(0,0,rot));
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
        GameObject attack = Instantiate(prefabLib.chillingBurstVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(7, 7) * playerStats.GetAbilitySizeMod();
        yield return new WaitForSeconds(0.3f);
        Destroy(attack);
    }
    private IEnumerator FlashBangTimer()
    {
        RaycastHit2D[] hits = MakeBoxCastAll("flashbang");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod(), playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod());
            }  
        }

        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        GameObject attack = Instantiate(prefabLib.flashbangvisual, transform.position + (Vector3)angleAsVector, anchorTransform.rotation, transform);
        attack.transform.localScale = new Vector2(1, 1) * playerStats.GetAbilitySizeMod();
        yield return new WaitForSeconds(0.2f);
        Destroy(attack);
    }
    private IEnumerator SplinterCarbineTimer()
    {
        float timeBetweenBullets = 0.15f;
        for(int i = 0; i < 5; i++)
        {
            ActivateSplinterCarbine();
            yield return new WaitForSeconds(timeBetweenBullets);
        }
    }
    private IEnumerator PiercingDuetTimer()
    {
        float timeBetweenBullets = 0.15f;
        for(int i = 0; i < 33; i++)
        {
            ActivatePiercingDuet();
            yield return new WaitForSeconds(timeBetweenBullets);
        }
    }
    private IEnumerator ShockingWheelTimer()
    {
        canUseAbility1 = false;
        RaycastHit2D[] hits = MakeCircleCastAll("shockingwheel");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod(), playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod() * 1.5f);
            }  
        }
        GameObject attack = Instantiate(prefabLib.shockingWheelVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(8, 8) * playerStats.GetAbilitySizeMod();
        yield return new WaitForSeconds(0.3f);
        Destroy(attack);
    }
    private IEnumerator FreezingWheelTimer()
    {
        canUseAbility1 = false;
        RaycastHit2D[] hits = MakeCircleCastAll("shockingwheel");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod(), playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod() * 1.5f);
                enemy.enemyStats.AddEffect("frozen", 3);
            }  
        }
        GameObject attack = Instantiate(prefabLib.freezingWheelVisual, transform.position, quaternion.Euler(Vector3.zero), transform);
        attack.transform.localScale = new Vector2(8, 8) * playerStats.GetAbilitySizeMod();
        yield return new WaitForSeconds(0.3f);
        Destroy(attack);
    }
    private IEnumerator CombustionCarbineTimer()
    {
        float timeBetweenBullets = 0.15f;
        for(int i = 0; i < 5; i++)
        {
            ActivateCombustionCarbine();
            yield return new WaitForSeconds(timeBetweenBullets);
        }
    }
    #endregion
    // hearts
    private IEnumerator HyperDashTimer()
    {
        hyperDash = true;
        yield return new WaitForSeconds(3);
        hyperDash = false;
    }
    private IEnumerator AccultSacrificeTimer()
    {
        int initialKills = playerStats.kills;
        yield return new WaitForSeconds(6);
        int finalKills = playerStats.kills;
        GetHealed(10*(finalKills-initialKills));
    }
    private IEnumerator ShieldingWardTimer()
    {
        playerStats.UpdateMaxHealth(30);
        playerStats.Heal(30);
        yield return new WaitForSeconds(10);
        playerStats.UpdateMaxHealth(-30);
        playerStats.TakeDamage(15);
    }
    // clubs
    private IEnumerator UnyieldingChargeTimer()
    {
        isCharging = true;
        yield return new WaitForSeconds(4);
        isCharging = false;
    }
    private IEnumerator EarthBreakTimer()
    {
        RaycastHit2D[] hits1 = MakeBoxCastAll("rupture",Vector3.zero,0);
        RaycastHit2D[] hits2 = MakeBoxCastAll("rupture", Vector3.zero,45);
        RaycastHit2D[] hits3 = MakeBoxCastAll("rupture", Vector3.zero,-45);
        foreach (RaycastHit2D hit in hits1)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod() * 2, playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod());
            }
        }
        foreach (RaycastHit2D hit in hits2)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod() * 2, playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod());
            }
        }
        foreach (RaycastHit2D hit in hits3)
        {
            if (hit && hit.rigidbody.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.GetHitAway(this, playerStats.baseAbilityKnockback * playerStats.GetAbilityKnockbackMod() * 2, playerStats.baseAbilityDamage * playerStats.GetAbilityDamageMod());
            }
        }
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (4 * playerStats.GetAbilitySizeMod() / 2 + 1);
        Vector3 rotation = anchorTransform.rotation.eulerAngles + new Vector3(0,0,0);
        GameObject attack1 = Instantiate(prefabLib.groundRupture, transform.position + (Vector3)position, Quaternion.Euler(rotation), transform);
        attack1.transform.localScale = new Vector2(1.5f, 4) * playerStats.GetAbilitySizeMod();

        angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * (attackAngle+45)), Mathf.Cos(Mathf.Deg2Rad * (attackAngle + 45)));
        position = angleAsVector * (4 * playerStats.GetAbilitySizeMod() / 2 + 1);
        rotation = anchorTransform.rotation.eulerAngles + new Vector3(0,0,45);
        GameObject attack2 = Instantiate(prefabLib.groundRupture, transform.position + (Vector3)position, Quaternion.Euler(rotation), transform);
        attack2.transform.localScale = new Vector2(1.5f, 4) * playerStats.GetAbilitySizeMod();

        angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * (attackAngle - 45)), Mathf.Cos(Mathf.Deg2Rad * (attackAngle - 45)));
        position = angleAsVector * (4 * playerStats.GetAbilitySizeMod() / 2 + 1);
        rotation = anchorTransform.rotation.eulerAngles + new Vector3(0,0,-45);
        GameObject attack3 = Instantiate(prefabLib.groundRupture, transform.position + (Vector3)position, Quaternion.Euler(rotation), transform);
        attack3.transform.localScale = new Vector2(1.5f, 4) * playerStats.GetAbilitySizeMod();
        
        yield return new WaitForSeconds(0.1f);
        Destroy(attack1);
        Destroy(attack2);
        Destroy(attack3);

    }
    // not abilities
    private IEnumerator Ability1Timer()
    {
        canUseAbility1 = false;
        yield return new WaitForSeconds(1*playerStats.GetAbilityCooldownMod());
        if(playerStats.passiveAbility1.code == "b7d")
            yield return new WaitForSeconds(5);
        if (playerStats.passiveAbility1.code == "b3h")
            yield return new WaitForSeconds(3);
        if(playerStats.passiveAbility1.code == "b7h")
            yield return new WaitForSeconds(6);
        if(playerStats.passiveAbility1.code == "n8h")
            yield return new WaitForSeconds(10);
        if (playerStats.passiveAbility1.code == "b8c")
            yield return new WaitForSeconds(4);

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
                if(playerStats.passiveAbility1.code == "n5h")
                {
                    playerStats.Heal(playerStats.weapon.baseAttack * playerStats.GetAttackDamageMod() * 0.1f);
                }
            }
            
        }
        // makes an attack visual sprite when using a melee attack
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()/2+1);
        if (isParrying) position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()+1);
        GameObject attack = Instantiate(prefabLib.attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
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
                enemies.GetHit(this, 0, playerStats.weapon.baseAttack*playerStats.GetAttackDamageMod()*2);
                enemies.enemyStats.AddEffect("stun",3);
            }
        }
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()/2+1);
        if (isParrying) position = angleAsVector * (playerStats.weapon.baseAttackSize.y*playerStats.GetAttackSizeMod()+1);
        GameObject attack = Instantiate(prefabLib.attackVisual, transform.position + (Vector3)position, anchorTransform.rotation, transform);
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
        if(hyperDash) yield return new WaitForSeconds(playerStats.baseDashCooldown * playerStats.GetDashCooldownMod() *0.2f);
        else yield return new WaitForSeconds(playerStats.baseDashCooldown * playerStats.GetDashCooldownMod());
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
    private IEnumerator GetHealedTimer()
    {
        spriteRend.color = new Color32(0,150,0,255);
        yield return new WaitForSeconds(0.1f);
        spriteRend.color = currentColor;
    }
    private IEnumerator ParryTimer()
    {
        canParry = false;
        isParrying = true;
        // makes the parry visual when parrying
        Vector2 angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle), Mathf.Cos(Mathf.Deg2Rad * attackAngle));
        Vector2 position = angleAsVector * (playerStats.weapon.baseParrySize.y/2+1);
        GameObject parry = Instantiate(prefabLib.parryObject, transform.position + (Vector3)position, anchorTransform.rotation, anchorTransform);
        parry.transform.localScale = playerStats.weapon.baseParrySize;
        // to here
        yield return new WaitForSeconds(0.1f);
        Destroy(parry);
        yield return new WaitForSeconds(playerStats.baseParryTime-0.1f);
        isParrying = false;
        yield return new WaitForSeconds(playerStats.baseParryCooldown);
        canParry = true;
    }
    private IEnumerator DiamondIndicatorTimer()
    {
        GameObject indicator = Instantiate(prefabLib.diamondIndicator, transform.position, prefabLib.diamondIndicator.transform.rotation, transform);
        yield return new WaitForSeconds(0.3f);
        Destroy(indicator);
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
    private void CheckCurrentColor()
    {
        Color32 setColor = baseColor;
        if (playerStats.hasCharm)
            setColor = CombineColors(setColor, new Color32(255, 70, 190, 255));
        if (playerStats.hasChill)
            setColor = CombineColors(setColor, new Color32(80, 190, 255, 255));
        if (playerStats.hasFrozen)
            setColor = CombineColors(setColor, new Color32(170, 220, 255, 255));
        if (playerStats.hasPoison)
            setColor = CombineColors(setColor, new Color32(50, 220, 70, 255));
        currentColor = setColor;
    }
    private Color32 CombineColors(Color32 color1, Color32 color2)
    {
        int r = (color1.r + color2.r) / 2;
        int g = (color1.g + color2.g) / 2;
        int b = (color1.b + color2.b) / 2;
        return new Color32((byte)r, (byte)g, (byte)b, 255);
    }
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
        else if (type == "rupture")
        {
            angleAsVector = new(-Mathf.Sin(Mathf.Deg2Rad * attackAngle + rot), Mathf.Cos(Mathf.Deg2Rad * attackAngle + rot));
            Vector2 position = angleAsVector * (4 * playerStats.GetAbilitySizeMod() / 2 + 1);
            return Physics2D.BoxCastAll(transform.position + (Vector3)position, new Vector2(1.5f, 4) * playerStats.GetAbilitySizeMod(), attackAngle + rot, Vector2.zero, 0, attackLayer);

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
        else if (type == "shockingwheel")
        {
            return Physics2D.CircleCastAll(transform.position, 4f, Vector2.zero, 0, attackLayer);
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
