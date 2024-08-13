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

        if (playerInRange && !Attacking)
        {
            if (inMeleeRange)
            {
                if (!Attacking) // Ensure the attack isn't already started
                {
                    StartCoroutine(MeleeAttack());
                }
            }
            else
            {
                // Resume chasing if player leaves melee range but is still in aggro range
                agent.isStopped = false;
                agent.SetDestination(GameManager.instance.player.transform.position);
            }
        }
    }

    IEnumerator MeleeAttack()
    {
        Attacking = true;
        agent.isStopped = true; // Stop moving while attacking

        // Wait to simulate attack rate delay
        yield return new WaitForSeconds(atkRate / 2);

        // Check again if the player is still in melee range before dealing damage
        Collider enemyCollider = GetComponent<CapsuleCollider>();
        Collider playerCollider = GameManager.instance.player.GetComponent<Collider>();

        if (enemyCollider.bounds.Intersects(playerCollider.bounds))
        {
            // Apply damage if still in range
            GameManager.instance.player.GetComponent<PlayerController>().TakeDamage(dmg);
        }

        // Finish attack
        yield return new WaitForSeconds(atkRate / 2);

        Attacking = false;
        agent.isStopped = false;

        // Resume chasing if player is out of melee range
        if (!enemyCollider.bounds.Intersects(playerCollider.bounds))
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }
}