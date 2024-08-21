using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrenadePickUp : MonoBehaviour
{
    public GameObject GrenadeOnPlayer;
    public TMP_Text PickUpText;
    // Start is called before the first frame update
    void Start()
    {
        // Initially hide the grenade on the player
        GrenadeOnPlayer.SetActive(false);
        PickUpText.enabled = false;
    }

    public void OnTriggerStay(Collider other) // OnTriggerStay is called continuously while the collider of this object is touching another object
    {
        // Check if the other object is tagged as "Player"
        if (other.gameObject.tag == "Player")
        {
            PickUpText.enabled = true;

            // Check if the player is pressing the 'E' key
            if (Input.GetKey(KeyCode.E))
            {
                // Hide this grenade pickup object
                this.gameObject.SetActive(false);

                // Show the grenade on the player
                GrenadeOnPlayer.SetActive(true);

                PickUpText.enabled = false;
            }
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        //removes PickUp text from screen 
        if (PickUpText != null)
        {
            PickUpText.enabled = false;
        }
    }
}
