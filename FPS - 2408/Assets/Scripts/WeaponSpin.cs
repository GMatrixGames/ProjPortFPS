using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    [SerializeField] bool rotateOnZ;

    // Update is called once per frame
    void Update()
    {
        if (!rotateOnZ)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}
