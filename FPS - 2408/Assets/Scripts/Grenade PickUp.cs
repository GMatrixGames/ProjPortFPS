using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrenadePickUp : MonoBehaviour
{
    public GameObject GrenadeOnPlayer;
    private bool isPlayerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initially hide the grenade on the player
        GrenadeOnPlayer.SetActive(false);
        
    }

    public void OnTriggerStay(Collider other) // OnTriggerStay is called continuously while the collider of this object is touching another object
    {
        // Check if the other object is tagged as "Player"
        if (other.gameObject.tag == "Player")
        {
            // Update the UI text to prompt the player to pick up the grenade
            if (!isPlayerInRange)
            {
                GameManager.instance.UpdateGrenadeInteractText("Press E to pick up the grenade");
                isPlayerInRange = true;
            }
            // Check if the player is pressing the 'E' key
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Hide this grenade pickup object
                this.gameObject.SetActive(false);

                // Show the grenade on the player
                GrenadeOnPlayer.SetActive(true);

                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.PickUpGrenade();
                    GrenadeOnPlayer.SetActive(true);
                }

                // Clear the interact text
                GameManager.instance.UpdateGrenadeInteractText("");
            }
        }    
    }

    // This method is called when the player leaves the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Clear the interact text when the player leaves the pickup area
            GameManager.instance.UpdateGrenadeInteractText("");
            isPlayerInRange = false;
        }
    }
}
