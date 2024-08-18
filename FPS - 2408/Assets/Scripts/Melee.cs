using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : EnemyAI
{
    private bool Attacking;

    public new void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        base.Update();

        // Get colliders
        Collider enemyCollider = GetComponent<CapsuleCollider>();
        Collider playerCollider = GameManager.instance.player.GetComponent<Collider>();

        // Check if player is in melee range
        bool inMeleeRange = enemyCollider.bounds.Intersects(playerCollider.bounds);

        if (playerInRange)
        {
            if (inMeleeRange && !Attacking)
            {
                // Start the attack if in melee range
                StartCoroutine(MeleeAttack());
            }
            else if (!Attacking)
            {
                // Resume chasing if not in melee range and not attacking
                agent.isStopped = false;
                agent.SetDestination(GameManager.instance.player.transform.position);
            }
        }
        else
        {
            // Stop the enemy if the player is out of aggro range
            agent.isStopped = true;
        }
    }

    IEnumerator MeleeAttack()
    {
        Attacking = true;
        agent.isStopped = true; // Stop moving while attacking

        // Wait to simulate attack rate delay
        yield return new WaitForSeconds(atkRate);

        // Check again if the player is still in melee range before dealing damage
        Collider enemyCollider = GetComponent<CapsuleCollider>();
        Collider playerCollider = GameManager.instance.player.GetComponent<Collider>();

        if (enemyCollider.bounds.Intersects(playerCollider.bounds))
        {
            // Apply damage if still in range
            GameManager.instance.player.GetComponent<PlayerController>().TakeDamage(dmg);
        }

        // Finish attack
        yield return new WaitForSeconds(atkRate);

        Attacking = false;

        // Always resume chasing after the attack if the player is still in aggro range
        if (playerInRange)
        {
            agent.isStopped = false;
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }
}