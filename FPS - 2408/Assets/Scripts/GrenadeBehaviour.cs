using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f; 
    [SerializeField] private int explosionDamage = 50; 
    [SerializeField] private float explosionDelay = 3f;
    [SerializeField] private ParticleSystem ExplosionEffect;
    [SerializeField] private AudioClip explosionSound;

    private bool isReadyToExplode;

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
            StartCoroutine(ExplodeAfterDelay());
        }
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        Debug.Log("Grenade exploded.");

        if (ExplosionEffect != null)
        {
            var effect = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 1f);

            GetComponent<AudioSource>().PlayOneShot(explosionSound);
        }
        else
        {
            Debug.LogWarning("ExplosionEffect is not assigned.");
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                continue;
            }

            IDamage damageable = collider.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }
        }
        StartCoroutine(DestroyAfterEffect());
    }
   
    private IEnumerator DestroyAfterEffect()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}