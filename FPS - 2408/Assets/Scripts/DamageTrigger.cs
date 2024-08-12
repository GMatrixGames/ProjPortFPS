using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private enum DamageType
    {
        Bullet,
        Stationary
    }

    [SerializeField] private DamageType type;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] private int destroyTime;
    [SerializeField] private float headShotMultiplier = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (type == DamageType.Bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        var dmg = other.GetComponent<IDamage>();
        dmg?.TakeDamage(damage);

        if(dmg != null)
        {
            if(other.CompareTag("Head"))
            {
                dmg.TakeDamage(damage * (int)headShotMultiplier);
            }
            else
            {
                dmg.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}