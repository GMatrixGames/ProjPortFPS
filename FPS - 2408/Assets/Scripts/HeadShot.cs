using System.Collections;
using UnityEngine;

public class HeadShot : MonoBehaviour
{
    [SerializeField] private string headTag = "Head"; // The tag used to identify the head collider.

    // Method to apply damage, checking if it's a headshot or not.
    public void ApplyDamage(int baseDamage, Collider hitCollider)
    {
        IDamage damageable = GetComponent<IDamage>(); // Get the IDamage component from the same GameObject.

        if (damageable != null)
        {
            // Check if the hit collider has the specified head tag.
            if (hitCollider.CompareTag(headTag))
            {
                Debug.Log("Headshot detected! Applying one-shot kill.");
                int oneShotDamage = 9999; // Using a very high damage value to guarantee a kill.
                damageable.TakeDamage(oneShotDamage);
            }
            else
            {
                Debug.Log("Body shot detected, applying regular damage.");
                damageable.TakeDamage(baseDamage);
            }
        }
    }
}