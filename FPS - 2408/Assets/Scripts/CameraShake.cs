using System.Collections;
using UnityEngine;

// Controls the shaking effect of the camera in response to game events, such as when the player takes damage.
public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f; // The duration of the shake effect in seconds.
    [SerializeField] private float shakeMagnitude = 0.2f; // The magnitude of the shake, affecting how much the camera moves.

    // Triggers the camera shake whenever the player takes damage.
    public void TriggerShake()
    {
        StartCoroutine(Shake());
    }

    // Performs the actual shaking of the camera by randomly adjusting its position within a certain range defined by shakeMagnitude.
    private IEnumerator Shake()
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

        // Reset the camera's position to its original state after the shake.
        transform.localPosition = originalPosition;
    }
}
