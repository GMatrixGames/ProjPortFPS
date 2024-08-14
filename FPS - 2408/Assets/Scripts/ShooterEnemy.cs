using System.Collections;
using UnityEngine;

public class ShooterEnemyAI : EnemyAI
{
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootRate;

    private bool isShooting;

    protected override void AttackLogic()
    {
        if (!isShooting && CanSeePlayer())
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
