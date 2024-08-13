using System.Collections;
using UnityEngine;


// Controls the shaking effect of the camera in response to game events, such as when the player's health falls below a certain threshold.

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f; // The duration of the shake effect in seconds.
    [SerializeField] private float shakeMagnitude = 0.2f; // The magnitude of the shake, affecting how much the camera moves.
    [SerializeField] private int healthThreshold; // The health value below which the camera will shake.
    private bool shakeTriggered = false; // A flag to ensure the shake is only triggered once per event.


    // Triggers the camera shake if the player's health is below the specified threshold and the shake has not been triggered yet.
    // The current health of the player.
    public void TriggerShake(int currentHealth)
    {
        if (currentHealth <= healthThreshold && !shakeTriggered)
        {
            StartCoroutine(Shake());
            shakeTriggered = true; // Set the flag to prevent multiple shakes from overlapping.
        }
    }


    // Performs the actual shaking of the camera by randomly adjusting its position within a certain range defined by shakeMagnitude.
    public IEnumerator Shake()
    {
        Vector3 originalPosition = transform.localPosition; // Store the original position of the camera.
        float elapsed = 0.0f; // Timer to track the duration of the shake.

        while (elapsed < shakeDuration)
        {
            // Generate random offsets for x and y axes based on shakeMagnitude.
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = Random.Range(-shakeMagnitude, shakeMagnitude);

            // Apply the shake effect by moving the camera's position.
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime; // Increment the timer.
            yield return null; // Wait until the next frame before continuing the loop.
        }

        transform.localPosition = originalPosition; // Reset the camera's position to its original state after the shake.
    }


    // Resets the shake trigger if the player's health increases above the threshold after the shake has been triggered.
    // This allows the shake to be triggered again if the conditions are met.
    // The current health of the player to check against the threshold.
    public void ResetShakeTrigger(int currentHealth)
    {
        if (currentHealth > healthThreshold)
        {
            shakeTriggered = false; // Reset the flag to enable re-triggering the shake.
        }
    }
}
