using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private int damageAmount;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        var dmg = other.GetComponent<IDamage>();
        dmg?.TakeDamage(damageAmount);
    }
}