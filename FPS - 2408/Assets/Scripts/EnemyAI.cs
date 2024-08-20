using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Renderer model;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform headPos;

    [SerializeField] private int hp;
    private int maxHp;
    [SerializeField] private int viewAngle;
    [SerializeField] private int facePlayerSpeed;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootRate;

    private bool isShooting;
    private bool playerInRange;
    private bool isAttacking;

    private float angleToPlayer;

    private Vector3 playerDir;

    private Color colorOriginal;

    // Start is called before the first frame update
    private void Start()
    {
        maxHp = hp; // hp should initially be max
        colorOriginal = model.material.color;
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerInRange && CanSeePlayer())
        {
        }
    }

    private bool CanSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);

        if (Physics.Raycast(headPos.position, playerDir, out var hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                if (!isShooting) StartCoroutine(Shoot());
                if (agent.remainingDistance <= agent.stoppingDistance) FacePlayer();

                return true;
            }
        }

        return false;
    }

    private void FacePlayer()
    {
        var rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    /// <inheritdoc/>
    public void TakeDamage(int amount)
    {
        hp -= amount;

        StartCoroutine(FlashRed());

        if (hp <= 0)
        {
            hp = 0;

            // Increments the kill count in GameManager when this enemy dies
            GameManager.instance.UpdateGoal(1);

            Destroy(gameObject);
        }

        GameManager.instance.enemyHitText.text = $"Enemy: {hp}/{maxHp}";
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

    /// <summary>
    /// Shoot bullet at player.
    /// </summary>
    /// <returns>Delay</returns>
    private IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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