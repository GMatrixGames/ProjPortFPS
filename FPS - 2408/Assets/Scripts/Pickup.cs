using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private GunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // give the player the gun
            GameManager.instance.playerScript.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}