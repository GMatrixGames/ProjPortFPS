using UnityEngine;

public class GrenadePickUp : MonoBehaviour
{
    public int grenadesToAdd = 1;
    public void OnTriggerStay(Collider other) 
    {
        // Check if the other object is tagged as "Player"
        if (other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Thrown Grenade"))
        {
            // Hide this grenade pickup object
            gameObject.SetActive(false);

            var playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {                
                playerController.PickUpGrenade(grenadesToAdd);
            }
        }
    }
}
