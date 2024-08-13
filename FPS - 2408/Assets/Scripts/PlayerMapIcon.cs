using UnityEngine;

public class PlayerMapIcon : MonoBehaviour
{
    private Transform player; // Reference to the player's transform
    private RectTransform playerIcon; // Reference to the player arrow icon on the minimap

    void Start()
    {
        // Find the Player GameObject by name
        var playerObject = GameObject.Find("Player");
        if (!playerObject)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }

        // Find the PlayerArrow GameObject by name
        var iconObject = GameObject.Find("PlayerArrow");
        if (!iconObject)
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
        if (player && playerIcon)
        {
            // Update the position of the arrow on the minimap
            var newPosition = player.position;
            newPosition.y = transform.position.y; // Keep the arrow at the same height
            transform.position = newPosition;

            // Update the rotation of the arrow to match the player's rotation
            playerIcon.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }
    }
}