using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f; // Radius of the explosion
    [SerializeField] private int explosionDamage = 50; // Damage dealt by the explosion
    [SerializeField] private float explosionDelay = 3f; // Time before the grenade explodes
    [SerializeField] private GameObject ExplosionEffect; // Explosion effect prefab

    private bool isReadyToExplode = false;

    private void Start()
    {
        // Ensure that the grenade does not explode immediately
        Debug.Log("Grenade initialized with delay: " + explosionDelay);
    }

    public void ActivateExplosion()
    {
        if (!isReadyToExplode)
        {
            isReadyToExplode = true;
            Debug.Log("Grenade timer started with a delay of " + explosionDelay + " seconds.");
            Invoke("Explode", explosionDelay);
        }
        else
        {
            Debug.LogWarning("ActivateExplosion was called but grenade was already set to explode.");
        }
    }

    private void Explode()
    {
        Debug.Log("Explode method called."); // Check if this is printed

        if (ExplosionEffect != null)
        {
            Debug.Log("Instantiating explosion effect."); // Check if this is printed
            Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("ExplosionEffect is not assigned.");
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        Debug.Log($"Found {colliders.Length} colliders in range."); // Debug number of colliders found
        foreach (Collider collider in colliders)
        {
            IDamage damageable = collider.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Log collision events for debugging
        Debug.Log("Grenade collided with " + collision.collider.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Log trigger events for debugging
        Debug.Log("Grenade triggered by " + other.name);
    }
}