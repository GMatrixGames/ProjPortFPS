using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour, IDamage
{
    [SerializeField] private int spawnerHP;
    [SerializeField] private Renderer model;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject enemyToSpawn;
    [SerializeField] private int maxEnemiesToSpawn;
    [SerializeField] private int timeBetweenSpawns;
    [SerializeField] private int distanceToSpawn;
    [SerializeField] private Image hpBar;

    private int hp;
    private int enemiesOnField;
    private bool hasSpawnedRecently;
    private bool isInsideRadius;

    private Color colorOriginal;

    private void Start()
    {
        hp = spawnerHP;
        hpBar.fillAmount = 1;
        colorOriginal = model.material.color;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isInsideRadius && hasSpawnedRecently == false && enemiesOnField < maxEnemiesToSpawn)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        hasSpawnedRecently = true;
        yield return new WaitForSeconds(timeBetweenSpawns);

        if (enemiesOnField < maxEnemiesToSpawn)
        {
            var randomPos = spawnPosition.position + Random.insideUnitSphere * distanceToSpawn;
            Instantiate(enemyToSpawn, randomPos, enemyToSpawn.transform.rotation);
            enemiesOnField++;
        }

        hasSpawnedRecently = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        isInsideRadius = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInsideRadius = false;
    }

    /// <summary>
    /// Flash red when taking damage.
    /// </summary>
    /// <returns>Delay</returns>
    private IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
    
    public void TakeDamage(int amount)
    {
        hp -= amount;
        hpBar.fillAmount = (float) hp / spawnerHP;

        StartCoroutine(FlashRed());

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}