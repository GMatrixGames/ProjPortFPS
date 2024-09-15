using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private enum DamageType
    {
        Bullet,
        Stationary,
        Melee
    }

    [SerializeField] private DamageType type;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] private int destroyTime;

    // Start is called before the first frame update
    private void Start()
    {
        if (type == DamageType.Bullet)
        {
            rb.velocity = (GameManager.instance.player.transform.position - (transform.position - new Vector3(0, 0.5f, 0))).normalized * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        var dmg = other.GetComponent<IDamage>();
        if (dmg != null && other.CompareTag("Player"))
        {
            dmg.TakeDamage(damage);
            if (type != DamageType.Stationary)
            {
                Destroy(gameObject);
            }
        }
    }
}