using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : EnemyAI
{
    // The amount of damage caused by a melee attack
    [SerializeField] int meleeDamage = 20;

    // The range at which the boss can perform melee attacks
    [SerializeField] float meleeRange = 3f;

    // Cooldown time for melee attacks
    [SerializeField] private float meleeAttackRate = 1.5f;

    [SerializeField] float shootRange = 25f;
    // Health threshold for phase change
    [SerializeField] float transitionHealth = 50;

    // The Speed of which the enemy will roll
    [SerializeField] float rollSpeed = 10f;

    // Cooldown time between rolls
    [SerializeField] float rollCooldown = 5f;

    private PlayerController playerController;

    [SerializeField] AudioClip rollSound;
    [SerializeField] float rollVol = 0.5f;

    // Track if the boss is in a melee phase
    private bool inMeleePhase = false;

    // Control taunting frequency
    private bool taunt = false;

    // Control roll usage
    private bool canRoll = true;

    // Is the enemy rolling
    private bool isRolling = false;

    // is the boss currentlt attacking
    private bool isAttacking = false;

    private Coroutine shootCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Calls the Start method from EnemyAI class
        base.Start();

        // Get the PlayerController component from the player GameObject
        PlayerController playerController = GameManager.instance.player.GetComponent<PlayerController>();

        StartCoroutine(TauntAtStart());
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Keeps the original enemy behaviors like roaming and shooting
        base.Update();

        // Update NavMeshAgent destination
        agent.SetDestination(GameManager.instance.player.transform.position);

        // If the boss's health falls low enough enter melee mode
        if (hp <= transitionHealth && !inMeleePhase)
        {
            // Enter melee attack
            EnterMeleeAttack();
        }

        // Handle melee attacks based on player distance
        HandleMeleeAttack();

        // Checks the player's distance to see if in shooting range or melee range
        RangedCombat();
    }

    #region Melee Combat
    // Focus on melee attacks when health is at a certain level
    void EnterMeleeAttack()
    {
        // Puts boss into melee phase
        inMeleePhase = true;
    }

    // Checks if the player is in melee range and initiates an attack
    void HandleMeleeAttack()
    {
        // Prevents melee if the boss is rolling
        if (isRolling || isShooting)
        {
            return;
        }
        // Check if the player is within melee range
        if (playerInRange && Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= meleeRange)
        {
            // Ensure the boss faces the player before attacking
            FacePlayer();

            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                isShooting = false;
            }

            // Chooses between the 2 melee attack types
            int attackType = Random.Range(1, 3);

            // If the boss is not already shooting, initiate a melee attack
            if (!isAttacking)
            {
                isAttacking = true;
                agent.isStopped = true;

                // Sets the attack type in animator
                anim.SetInteger("Type", attackType);
                StartCoroutine(MeleeAttack());
            }
        }
    }

    // Handles the melee attack, adding a delay before the attack hits
    IEnumerator MeleeAttack()
    {
        // starts melee attack
        isAttacking = true;

        // Prevent multiple attacks at once
        isShooting = true;

        // Trigger the melee attack animation
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        // After the delay, apply the melee damage to the player
        MeleeDamage();

        // Wait for the shootRate (cooldown) before allowing another melee attack
        yield return new WaitForSeconds(meleeAttackRate);
        isShooting = false;
        agent.isStopped = false;
    }

    // Applies melee damage to the player
    void MeleeDamage()
    {

        // Check if the player has a PlayerController component before applying damage
        if (playerController != null)
        {
            // Call the TakeDamage method on the player's PlayerController script to apply melee damage
            playerController.TakeDamage(meleeDamage);
        }
    }
    #endregion

    #region Ranged Combat
    void RangedCombat()
    {
        // Calculates the distance between the boss and the player
        float distToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // If the boss is already attacking don't do any other attacks
        if (isAttacking)
        {
            return;
        }

        // If the player is witin melee range switch to melee attack
        if (distToPlayer <= meleeRange)
        {
            HandleMeleeAttack();
        }

        // If the player is within shoot range switch to ranged combat
        else if (distToPlayer <= shootRange)
        {
            // Trigger Shoot logic
            HandleShooting();
        }
    }

    // Shoots when the player is in ranged combat
    void HandleShooting()
    {
        float distToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // Only shoot when the player is outside melee range
        if (distToPlayer > meleeRange && !isShooting && !isAttacking)
        {
            // Boss will face player
            FacePlayer();

            // Trigger Shooting logic
            shootCoroutine = StartCoroutine(ShootPlayer());
        }
    }

    // To handle shooting the player
    IEnumerator ShootPlayer()
    {
        // Starts ranged attack
        isShooting = true;

        // Trigger shoot animation
        anim.SetTrigger("Shoot");

        // Wait for the shootRate(cooldown) before allowing another ranged attack
        yield return new WaitForSeconds(shootRate);

        // Ranged attack finished
        isShooting = false;
    }
    #endregion

    #region Roll
    // Handle roll behavior
    IEnumerator HandleRoll()
    {
        // Will keep the enemy from doing multiple rolls
        canRoll = false;

        // Begin rolling
        isRolling = true;


        // Trigger roll animation
        anim.SetTrigger("Roll");

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        // Move the boss forward quicky during the roll
        Vector3 rollDirection = transform.forward;
        float rollDuration = 0.1f; // Duration of the roll animation
        float elapsedTime = 0f;

        // Does roll movement 
        while (elapsedTime < rollDuration)
        {
            transform.Translate(rollDirection * Time.deltaTime * rollSpeed, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Ends rolling
        isRolling = false;

        // Roll cooldown
        yield return new WaitForSeconds(rollCooldown);

        // After cooldown time the enemy is able to roll again
        canRoll = true;
    }

    public void EndRoll()
    {
        isRolling = false;
        canRoll = false;
    }

    // Handles if the boss should roll
    bool BossShouldRoll()
    {
        // The boss will roll if the player is within 6 units of the player
        return Vector3.Distance(transform.position, GameManager.instance.player.transform.position) < 6f;
    }

    // Plays sound when enemy rolls
    public void RollSound()
    {
        if (rollSound != null)
        {
            AudioSource.PlayClipAtPoint(rollSound, transform.position, rollVol);
        }
    }
    #endregion

    #region Damage
    // Override the TakeDamage method to handle boss-specific damage logic
    public override void TakeDamage(int amount)
    {
        // Base class that handles standard damage
        base.TakeDamage(amount);

        if (hp <= 0)
        {
            // Trigger the death animation
            anim.SetTrigger("Dead");
            Destroy(gameObject);
        }
        else
        {
            // Trigger a damage animation
            DamageAnimation();
        }
    }

    // Randomly selects one of the damage animations
    void DamageAnimation()
    {
        // Randomly selects between 1 and 3
        int damageType = Random.Range(1, 4);
        switch (damageType)
        {
            case 1:
                anim.SetTrigger("DamageA");
                break;
            case 2:
                anim.SetTrigger("DamageB");
                break;
            case 3:
                anim.SetTrigger("DamageC");
                break;
        }
    }
    #endregion

    #region Taunt
    // Boss will taunt at the beginning of the battle
    IEnumerator TauntAtStart()
    {
        if (!taunt)
        {
            // Stop the boss's movement
            agent.isStopped = true;

            // Trigger taunt animation
            anim.SetTrigger("Taunt");

            // Prevents taunting again
            taunt = true;
            yield return new WaitForSeconds(1f);

            // Allows the boss to move after taunt is finished
            agent.isStopped = false;
        }
    }
    #endregion
}

