using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private enum DamageType
    {
        Bullet,
        Stationary
    }

    [SerializeField] private DamageType type;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] private int destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        if (type == DamageType.Bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        var dmg = other.GetComponent<IDamage>();
        dmg?.TakeDamage(damage);
        
        Destroy(gameObject);
    }
}