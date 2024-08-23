using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Transform spawnPosition;
    [SerializeField] GameObject enemyToSpawn;
    [SerializeField] int maxEnemiesToSpawn;
    [SerializeField] int timeBetweenSpawns;

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
            Instantiate(enemyToSpawn, spawnPosition);
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

}

