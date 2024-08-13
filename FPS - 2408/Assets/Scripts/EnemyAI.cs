using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Renderer model;
    [SerializeField] private Transform throwPos;

    [SerializeField] private int hp;

    [SerializeField] private GameObject grenade;

    [SerializeField] public int atkRate;
    [SerializeField] public int dmg;

    [SerializeField] bool isGrenadier;
    [SerializeField] float throwCooldown;
    [SerializeField] float throwForce;

    public bool playerInRange;
    public bool isThrowing;

    public Color colorOriginal;

    // Start is called before the first frame update
    public void Start()
    {
        colorOriginal = model.material.color;
        GameManager.instance.UpdateGoal(1);
    }

    // Update is called once per frame
    public void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
            if (!isThrowing)
            {
                StartCoroutine(ThrowGrenade());
            }
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

    // Grenade throw
    IEnumerator ThrowGrenade()
    {
        isThrowing = true;
        Instantiate(grenade);
        GrenadeBehavior grenadeScript = grenade.GetComponent<GrenadeBehavior>();
        if (grenadeScript != null)
        {
            Vector3 targetPosition = GameManager.instance.player.transform.position;
            grenadeScript.InitializeAndThrow(throwPos.position, targetPosition, throwForce);
        }
        yield return new WaitForSeconds(throwCooldown);
        isThrowing = false;
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