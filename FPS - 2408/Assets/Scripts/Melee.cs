using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : EnemyAI
{
    private bool isAttacking;  
    [SerializeField] private int meleeDamage;  
    [SerializeField] private float meleeRate;
    [SerializeField] private float meleeRange;

    private Collider playerCollider; 

    protected override void Start()
    {
        // Calls the base class Start method
        base.Start();  

        // Sets the stopping distance to prevent the enemy from getting too close
        agent.stoppingDistance = meleeRange;

        // Gets reference to the player's collider
        playerCollider = GameManager.instance.player.GetComponent<Collider>();

        // Disables the physical collision between the enemy and the player
        // Keeping the enemy from pushing the player around when attacking
        Physics.IgnoreCollision(playerCollider, GetComponent<Collider>());
    }

    protected override void Update()
    {
        // Calls the base class Update method
        base.Update(); 

        // Calculates the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // If the enemy is not already attacking and is within melee range
        if (!isAttacking && distanceToPlayer <= meleeRange)
        {
            // Stops the enemy from moving and begins the attack
            agent.isStopped = true;
            StartCoroutine(MeleeAttack());
        }
        // If the enemy is not attacking and is outside of melee range
        else if (!isAttacking && distanceToPlayer > meleeRange)
        {
            // Continue moving towards the player while maintaining the stopping distance
            agent.isStopped = false;
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }

    // Coroutine to handle melee attacks
    private IEnumerator MeleeAttack()
    {
        // Preventa multiple attacks at once
        isAttacking = true;

        // Makes sure the enemy stops moving during the attack
        agent.isStopped = true;  

        // Deal damage to the player
        GameManager.instance.player.GetComponent<PlayerController>().TakeDamage(meleeDamage);

        // Triggers the attack animation
        anim.SetTrigger("Attack");

        // Wait for the attack cooldown duration
        yield return new WaitForSeconds(meleeRate);

        // After the cooldown allow movement and set attack to false
        isAttacking = false;
        agent.isStopped = false;
    }
}
