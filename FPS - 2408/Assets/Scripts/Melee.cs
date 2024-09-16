using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : EnemyAI
{
    // Flag to check if the enemy is currently attacking
    private bool isAttacking;

    // Amount of damage the melee attack will deal
    [SerializeField] private int meleeDamage;

    // Time delay between attacks
    [SerializeField] private float meleeRate;

    // Range at which the enemy can perform melee attacks
    [SerializeField] private float meleeRange = 3;

    // Reference to the player's collider for distance checking
    private Collider playerCollider;

    // This function is called when the script is first run
    protected override void Start()
    {
        // Call the base class Start method to set up basic enemy behavior
        base.Start();

        // Set the stopping distance of the NavMeshAgent to the melee range
        agent.stoppingDistance = meleeRange;

        // Get the player's collider from the GameManager for future reference
        playerCollider = GameManager.instance.player.GetComponent<Collider>();
    }

    // This function is called every frame
    protected override void Update()
    {
        // Call the base class Update method to ensure normal enemy movement and behavior
        base.Update();

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // If the enemy is not attacking and is within melee range
        if (!isAttacking && distanceToPlayer <= meleeRange)
        {
            // Stops the enemy's movement and start the attack
            agent.isStopped = true;
            StartCoroutine(MeleeAttack());
        }
        // If the enemy is not attacking and is outside the melee range
        else if (!isAttacking && distanceToPlayer > meleeRange)
        {
            // Resume movement towards the player
            agent.isStopped = false;
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }

    // Coroutine that handles the melee attack
    private IEnumerator MeleeAttack()
    {
        // Set isAttacking to true to prevent multiple attacks at the same time
        isAttacking = true;

        // Stop the enemy from moving while the attack is happening
        agent.isStopped = true;

        // Apply damage to the player
        GameManager.instance.player.GetComponent<PlayerController>().TakeDamage(meleeDamage);

        // Trigger the attack animation
        anim.SetTrigger("Attack");

        // Wait for the time specified by meleeRate before the next attack
        yield return new WaitForSeconds(meleeRate);

        // After the cooldown, allow the enemy to move and attack again
        isAttacking = false;
        agent.isStopped = false;
    }
}
