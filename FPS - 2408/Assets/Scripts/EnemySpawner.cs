using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IDamage
{
    [SerializeField] private int spawnerHP;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject enemyToSpawn;
    [SerializeField] private int maxEnemiesToSpawn;
    [SerializeField] private int timeBetweenSpawns;
    [SerializeField] private int distanceToSpawn;

    private int enemiesOnField;
    private bool hasSpawnedRecently;
    private bool isInsideRadius;

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

    public void TakeDamage(int amount)
    {
        spawnerHP -= amount;

        if (spawnerHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}