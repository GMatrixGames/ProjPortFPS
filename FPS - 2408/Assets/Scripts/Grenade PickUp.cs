using UnityEngine;

public class GrenadePickUp : MonoBehaviour
{
    public void OnTriggerStay(Collider other) // OnTriggerStay is called continuously while the collider of this object is touching another object
    {
        // Check if the other object is tagged as "Player"
        if (other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Thrown Grenade"))
        {
            // Hide this grenade pickup object
            gameObject.SetActive(false);

            var playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.GrenadeOnPlayer.SetActive(true);
                playerController.PickUpGrenade();
            }
        }
    }
}
