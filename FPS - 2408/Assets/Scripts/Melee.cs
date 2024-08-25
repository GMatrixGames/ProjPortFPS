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
        var enemyCollider = GetComponent<Collider>();

        var inMeleeRange = enemyCollider.bounds.Intersects(playerCollider.bounds);

        if (CanSeePlayer() && inMeleeRange && !isAttacking)
        {
            StartCoroutine(MeleeAttack());
        }
        else if (CanSeePlayer() && !inMeleeRange && !isAttacking)
        {
            base.Update();
        }
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        agent.isStopped = true; // Stop moving while attacking

        // Check if the player is still in melee range before dealing damage
        var enemyCollider = GetComponent<Collider>();
        var inMeleeRange = enemyCollider.bounds.Intersects(playerCollider.bounds);

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