using System.Collections;
using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    [SerializeField] private float meleeRange = 2.0f;
    [SerializeField] private float attackRate = 1.0f;

    protected override void AttackLogic()
    {
        if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= meleeRange)
        {
            if (!isAttacking) StartCoroutine(AttackPlayer());
        }
        else
        {
            ChasePlayer();
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        // Example: Deal damage to the player
        // GameManager.instance.player.TakeDamage(attackDamage);
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }
}
