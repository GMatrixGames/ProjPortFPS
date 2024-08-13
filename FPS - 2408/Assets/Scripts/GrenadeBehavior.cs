using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehavior : MonoBehaviour
{
    [SerializeField] int delay = 3;
    [SerializeField] int explosionRadius = 5;
    [SerializeField] int explosionForce = 200;
    [SerializeField] int damage = 5;

    private bool hasExploded = false;

    public void InitializeAndThrow(Vector3 throwPosition, Vector3 targetPosition, float throwForce)
    {
        transform.position = throwPosition;
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = (targetPosition - throwPosition).normalized;
        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            IDamage damageable = nearbyObject.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
