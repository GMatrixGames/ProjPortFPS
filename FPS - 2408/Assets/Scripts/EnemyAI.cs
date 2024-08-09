using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private Renderer model;
    [SerializeField] private Transform shootPos;

    [SerializeField] private int HP;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootRate;

    private bool isShooting;

    private Color colorOriginal;

    // Start is called before the first frame update
    void Start()
    {
        colorOriginal = model.material.color;
        GameManager.instance.UpdateGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShooting)
            StartCoroutine(shoot());
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            GameManager.instance.UpdateGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}