using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : EnemyAI
{
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject bullet;

    private bool Shooting;

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();  // Call the base class's Start method
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();  // Call the base class's Update method

        if (playerInRange && !Shooting)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        Shooting = true;
        Instantiate(bullet, shootPosition.position, transform.rotation);
        yield return new WaitForSeconds(atkRate);
        Shooting = false;
    }
}
