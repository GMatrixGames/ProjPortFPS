using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] private Renderer model;

    [SerializeField] private int hp;

    [SerializeField] public int atkRate;
    [SerializeField] public int dmg;

    public bool playerInRange;

    public Color colorOriginal;

    // Start is called before the first frame update
    public void Start()
    {
        colorOriginal = model.material.color;
        GameManager.instance.UpdateGoal(1);
        GameManager.instance.UpdateKills(0);
    }

    // Update is called once per frame
    public void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }

    /// <inheritdoc/>
    public void TakeDamage(int amount)
    {
        hp -= amount;

        StartCoroutine(FlashRed());

        if (hp <= 0)
        {
            GameManager.instance.UpdateGoal(-1);
            Destroy(gameObject);
            GameManager.instance.UpdateKills(+1);
        }
    }

    /// <summary>
    /// Flash red when taking damage.
    /// </summary>
    /// <returns>Delay</returns>
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    /// <summary>
    /// When the player enters the enemy's range, set playerInRange to true.
    /// </summary>
    /// <param name="other">object entering the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    /// <summary>
    /// When the player exits the enemy's range, set playerInRange to false.
    /// </summary>
    /// <param name="other">object exiting the trigger</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}