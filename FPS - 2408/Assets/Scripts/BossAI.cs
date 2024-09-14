using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI
{
    // The amount of damage caused by a melee attack
    [SerializeField] int meleeDamage = 20;

    // The range at which the boss can perform melee attacks
    [SerializeField] float meleeRange = 3f;

    // Health threshold for phase change
    [SerializeField] float transitionHealth = 50;

    // Track if the boss is in a melee phase
    private bool inMeleePhase = false;

    // Control taunting frequency
    private bool canTaunt = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Calls the Start method from EnemyAI class
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Keeps the original enemy behaviors like roaming and shooting
        base.Update();

        // If the boss's health falls low enough enter melee mode
        if (hp <= transitionHealth && !inMeleePhase)
        {
            // Enter melee attack
            EnterMeleeAttack();
        }

        // Handle melee attacks based on player distance
        HandleMeleeAttack();

        // Boss taunts when transitioning to melee phase
        if (hp < 50 && canTaunt)
        {
            HandleTaunt();
        }
    }

    // Focus on melee attacks when health is at a certain level
    void EnterMeleeAttack()
    {
        // Puts boss into melee phase
        inMeleePhase = true;
    }

    // Checks if the player is in melee range and initiates an attack
    void HandleMeleeAttack()
    {
        // Check if the player is within melee range
        if (playerInRange && Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= meleeRange)
        {
            // Ensure the boss faces the player before attacking
            FacePlayer();

            // If the boss is not already shooting, initiate a melee attack
            if (!isShooting)
            {
                StartCoroutine(MeleeAttack());
            }
        }
    }

    // Handles the melee attack, adding a delay before the attack hits
    IEnumerator MeleeAttack()
    {
        // Prevent multiple attacks at once
        isShooting = true;

        // Trigger the melee attack animation
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        // After the delay, apply the melee damage to the player
        MeleeDamage();

        // Wait for the shootRate (cooldown) before allowing another melee attack
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    // Applies melee damage to the player
    void MeleeDamage()
    {
        // Get the PlayerController component from the player GameObject
        PlayerController playerController = GameManager.instance.player.GetComponent<PlayerController>();

        // Check if the player has a PlayerController component before applying damage
        if (playerController != null)
        {
            // Call the TakeDamage method on the player's PlayerController script to apply melee damage
            playerController.TakeDamage(meleeDamage);
        }
    }

    // Override the TakeDamage method to handle boss-specific damage logic
    public override void TakeDamage(int amount)
    {
        // Base class that handles standard damage
        base.TakeDamage(amount);

        if (hp <= 0)
        {
            // Trigger the death animation
            anim.SetTrigger("IsDead");
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

    // Taunting behavior logic
    void HandleTaunt()
    {
        if (canTaunt)
        {
            // Trigger the taunt animation
            anim.SetTrigger("Taunt");

            // Start a cooldown so the boss doesn't taunt too frequently
            StartCoroutine(TauntCooldown());
        }
    }

    IEnumerator TauntCooldown()
    {
        // Prevent taunting immediately again
        canTaunt = false;

        // Random time between 5 to 10 seconds
        yield return new WaitForSeconds(Random.Range(5f, 10f));

        // Allow the boss to taunt again
        canTaunt = true;
    }

}
