using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMapIcon : MonoBehaviour
{
    private Transform player;  // Reference to the player's transform
    private RectTransform playerIcon;  // Reference to the player arrow icon on the minimap

    void Start()
    {
        // Automatically find the player GameObject by name or tag
        GameObject playerObject = GameObject.Find("Player"); // Replace "Player" with the actual name of your player GameObject
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }

        // Automatically find the player icon by name or tag
        GameObject iconObject = GameObject.Find("PlayerArrow"); // Replace "PlayerArrow" with the actual name of your player arrow GameObject
        if (iconObject != null)
        {
            playerIcon = iconObject.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Player arrow icon not found!");
        }
    }

    //Update is called once per frame.
    private void Update()
    {
        if (player != null && playerIcon != null)
        {
            // Update the position of the arrow on the minimap
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y;  // Keep the arrow at the same height
            transform.position = newPosition;

            // Update the rotation of the arrow to match the player's rotation
            playerIcon.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }
    }
}
