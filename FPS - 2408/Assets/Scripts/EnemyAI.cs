using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Renderer model;
    [SerializeField] protected Animator anim;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform headPos;
    // [SerializeField] private Collider meleeCol;

    [SerializeField] private Image healthBar;
    [SerializeField] protected int hp;
    private int maxHp;
    [SerializeField] private int viewAngle;
    [SerializeField] private int facePlayerSpeed;
    [SerializeField] private int roamDistance;
    [SerializeField] private int roamTimer;
    [SerializeField] private int animSpeedTrans;

    [SerializeField] private GameObject bullet;
    [SerializeField] protected float shootRate;
    [SerializeField] private int shootAngle;

    protected bool isShooting;
    protected bool playerInRange;
    private bool isRoaming;

    private float angleToPlayer;
    private float stoppingDistanceOriginal;

    private Vector3 playerDir;
    private Vector3 startingPos;

    private Color colorOriginal;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        maxHp = hp; // hp should initially be max
        colorOriginal = model.material.color;
        healthBar.fillAmount = maxHp;
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPos = transform.position;

        GameManager.instance.UpdateEnemyMax(1);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        var agentSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animSpeedTrans));

        if (this is Melee) return; // Don't roam if this is a melee enemy

        if (playerInRange && !CanSeePlayer())
        {
            if (this is not BossAI && !isRoaming && agent.remainingDistance < 0.05)
            {
                StartCoroutine(Roam());
            }
        }
        else if (!playerInRange)
        {
            if (this is not BossAI && !isRoaming && agent.remainingDistance < 0.05)
            {
                StartCoroutine(Roam());
            }
        }
    }

    private IEnumerator Roam()
    {
        //isRoaming = true;

        //yield return new WaitForSeconds(roamTimer);
        //agent.stoppingDistance = 0;

        //var randomDist = Random.insideUnitSphere * roamDistance;
        //randomDist += startingPos;

        //if (NavMesh.SamplePosition(randomDist, out var hit, roamDistance, 1))
        //{
        //    agent.SetDestination(hit.position);
        //}

        //isRoaming = false;

        isRoaming = true;
        agent.stoppingDistance = 0; // Disable stopping distance while roaming

        float roamDuration = Random.Range(5f, 10f); // Random roaming time between 5 and 10 seconds
        float timeElapsed = 0f;

        Vector3 targetPosition = GetRandomRoamingPosition();

        while (timeElapsed < roamDuration)
        {
            if (agent.remainingDistance < 0.5f)
            {
                Vector3 newPosition = GetRandomRoamingPosition();
                
                if (Vector3.Distance(newPosition, targetPosition) > 1f)
                {
                    targetPosition = newPosition;
                    agent.SetDestination(targetPosition);
                }
            }
            
            if (agent.velocity.magnitude < 0.1f)
            {               
                targetPosition += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                agent.SetDestination(targetPosition);
            }

            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        isRoaming = false;
    }

    private Vector3 GetRandomRoamingPosition()
    {
        // Generate random position within roamDistance from the starting position
        Vector3 randomDir = Random.insideUnitSphere * roamDistance;
        randomDir += startingPos;

        // Ensure the new position is valid on the NavMesh and not too close to the player
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, roamDistance, NavMesh.AllAreas))
        {
            float distanceFromPlayer = Vector3.Distance(hit.position, GameManager.instance.player.transform.position);
            if (distanceFromPlayer >= 5f) 
            {
                return hit.position;
            }
        }

        return startingPos; // Fallback to starting position if no valid point found
    }
    protected bool CanSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // Debug.Log(angleToPlayer);
        //Debug.DrawRay(headPos.position, playerDir);

        if (Physics.Raycast(headPos.position, playerDir, out var hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                if (!isShooting && angleToPlayer <= shootAngle) StartCoroutine(Shoot());
                if (agent.remainingDistance <= agent.stoppingDistance) FacePlayer();

                agent.stoppingDistance = stoppingDistanceOriginal;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    protected void FacePlayer()
    {
        var rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    /// <inheritdoc/>
    virtual public void TakeDamage(int amount)
    {
        hp -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        StopCoroutine(Roam());

        StartCoroutine(FlashRed());

        healthBar.fillAmount = (float)hp / maxHp;

        if (hp <= 0)
        {
            // Increments the kill count in GameManager when this enemy dies
            GameManager.instance.UpdateEnemyGoal(1);

            if (spawner)
            {
                spawner.OnEnemyDeath(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
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
        anim.SetTrigger("Shoot");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    /// <summary>
    /// Create bullet at specific time in animation.
    /// </summary>
    public void CreateBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    private EnemySpawner spawner;

    public void SetSpawner(EnemySpawner newSpawner)
    {
        spawner = newSpawner;
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
            agent.stoppingDistance = 0;
        }
    }
}