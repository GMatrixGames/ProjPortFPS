using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenadier : EnemyAI
{
    [SerializeField] private Transform throwPos;
    [SerializeField] private GameObject grenade;
    [SerializeField] private int throwCooldown;
    private bool Throwing;

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();  // Call the base class's Start method
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();  // Call the base class's Update method

        if (playerInRange && !Throwing)
        {
            StartCoroutine(ThrowGrenade());
        }
    }

    // Grenade throw
    IEnumerator ThrowGrenade()
    {
        Throwing = true;
        Instantiate(grenade, throwPos.position, transform.rotation);
        yield return new WaitForSeconds(throwCooldown);
        Throwing = false;
    }
}
