using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : EnemyAI
{
    public bool Attacking;

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();  // Call the base class's Start method
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();  // Call the base class's Update method

        if (playerInRange && !Attacking)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    // Melee Attack
    IEnumerator MeleeAttack()
    {
        Collider enemyCollider = GetComponent<CapsuleCollider>();
        Collider playerCollider = GameManager.instance.player.GetComponent<Collider>();
        if (enemyCollider.bounds.Intersects(playerCollider.bounds))
        {
            agent.isStopped = true;
            Attacking = true;
            GameManager.instance.player.GetComponent<PlayerController>().TakeDamage(dmg);
            yield return new WaitForSeconds(atkRate);
            Attacking = false;
        }
        else
        {
            agent.isStopped = false;
        }
    }
}
