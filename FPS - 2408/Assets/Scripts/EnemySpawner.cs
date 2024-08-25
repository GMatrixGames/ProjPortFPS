using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IDamage
{
    [SerializeField] int spawnerHP;
    [SerializeField] Transform spawnPosition;
    [SerializeField] GameObject enemyToSpawn;
    [SerializeField] int maxEnemiesToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] int distanceToSpawn;

    public int enemiesOnField;
    bool hasSpawnedRecently;
    bool isInsideRadius;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInsideRadius == true && hasSpawnedRecently == false && enemiesOnField < maxEnemiesToSpawn)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
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

        if(spawnerHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}

