using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    private List<GameObject> spawnedEnemies = new();

    private Color colorOriginal;

    private void Start()
    {
        hp = spawnerHP;
        hpBar.fillAmount = 1;
        colorOriginal = model.material.color;
        GameManager.instance.UpdateSpawnersMax(1);
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
            if (NavMesh.SamplePosition(randomPos, out var hit, distanceToSpawn, NavMesh.AllAreas))
            {
                var enemy = Instantiate(enemyToSpawn, hit.position, enemyToSpawn.transform.rotation);
                enemy.GetComponent<EnemyAI>().SetSpawner(this);
                spawnedEnemies.Add(enemy);
                enemiesOnField++;
            }
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
            GameManager.instance.UpdateSpawnersGoal(1);
            Destroy(gameObject);
        }
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
            enemiesOnField--;
        }
    }
}