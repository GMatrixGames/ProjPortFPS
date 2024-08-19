using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Melee : EnemyAI
{
    private bool isAttacking;
    [SerializeField] private int meleeDamage;
    [SerializeField] private float meleeRate;
    private Collider playerCollider;

    private void Start()
    {
        playerCollider = GameManager.instance.player.GetComponent<Collider>();
    }

    public new void Update()
    {
        base.Update();
        Collider enemyCollider = GetComponent<Collider>();

        // Check if player is in melee range
        bool inMeleeRange = enemyCollider.bounds.Intersects(playerCollider.bounds);

        if (playerInRange && inMeleeRange && !isAttacking)
        {
            StartCoroutine(MeleeAttack());
        }
        else if (base.CanSeePlayer() && !inMeleeRange && !isAttacking)
        {
            base.Update();
        }
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        agent.isStopped = true; // Stop moving while attacking

        // Check if the player is still in melee range before dealing damage
        Collider enemyCollider = GetComponent<Collider>();
        bool inMeleeRange = enemyCollider.bounds.Intersects(playerCollider.bounds);

        if (inMeleeRange)
        {
            // Apply damage if still in range
            GameManager.instance.player.GetComponent<PlayerController>().TakeDamage(meleeDamage);
        }

        // Finish attack
        yield return new WaitForSeconds(meleeRate);

        // Resume movement
        agent.isStopped = false;
        isAttacking = false;
    }
}
