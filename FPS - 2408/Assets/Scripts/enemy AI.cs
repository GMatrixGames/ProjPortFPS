using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private Renderer model;

    [SerializeField] private int HP;

    Color colorOriginal;
    // Start is called before the first frame update
    void Start()
    {
        colorOriginal = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
}
