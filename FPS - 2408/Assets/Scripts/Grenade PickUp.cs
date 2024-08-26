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
        if (other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Thrown Grenade"))
        {
            isPlayerInRange = true;

            // Hide this grenade pickup object
            this.gameObject.SetActive(false);

            // Show the grenade on the player
            GrenadeOnPlayer.SetActive(true);

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PickUpGrenade();
                playerController.GrenadeOnPlayer = GrenadeOnPlayer;
            }
        }
    }
}
