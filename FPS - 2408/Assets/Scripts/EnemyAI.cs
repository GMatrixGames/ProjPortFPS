using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Renderer model;
    [SerializeField] protected Transform headPos;

    [SerializeField] protected int hp;
    [SerializeField] protected int viewAngle;
    [SerializeField] protected int facePlayerSpeed;

    protected bool playerInRange;
    protected bool isAttacking;

    protected float angleToPlayer;
    protected Vector3 playerDir;
    protected Vector3 startPosition;
    protected bool isChasing;
    protected bool isReturning;

    [SerializeField] protected float aggroDuration = 6.0f;
    protected float lastAggroTime;

    protected Color colorOriginal;

    protected virtual void Start()
    {
        colorOriginal = model.material.color;
        GameManager.instance.UpdateGoal(1);
        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (playerInRange && CanSeePlayer())
        {
            ChasePlayer();

            // This would be overridden or extended by subclasses like MeleeEnemyAI and ShooterEnemyAI
            AttackLogic();

            lastAggroTime = Time.time;
        }
        else
        {
            if (Time.time - lastAggroTime > aggroDuration)
            {
                if (!isReturning) StartCoroutine(ReturnToStartPosition());
            }
        }
    }

    protected virtual bool CanSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        if (Physics.Raycast(headPos.position, playerDir, out var hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                FacePlayer();

                return true;
            }
        }

        return false;
    }

    protected virtual void AttackLogic()
    {
        // This is intentionally left empty and should be overridden by subclasses
    }

    protected void ChasePlayer()
    {
        isChasing = true;
        isReturning = false;
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    protected IEnumerator ReturnToStartPosition()
    {
        isReturning = true;
        isChasing = false;
        agent.SetDestination(startPosition);
        while (Vector3.Distance(transform.position, startPosition) > agent.stoppingDistance)
        {
            yield return null;
        }
        isReturning = false;
    }

    protected void FacePlayer()
    {
        var rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

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

    protected IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            isChasing = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            isChasing = false;
        }
    }
}