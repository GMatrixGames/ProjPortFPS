using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Coroutine to shake the camera at a specified duration and magnitude
// Duration = How long the shake should last
// Magnitude = The intesity of the shake

public class CameraShake : MonoBehaviour
{
   public IEnumerator Shake(float duration, float magnitude)
    {
        // Stores the original position of the camera
        Vector3 origPos = transform.localPosition;

        float elasped = 0.0f;

        // This is the loop for the duration of the shake effect
        while(elasped < duration)
        {
            // This will give a random offset for the shake effect
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Applies the offset to the camera's position
            transform.localPosition = new Vector3(x, y, origPos.z);

            // Increments the elasped time
            elasped += Time.deltaTime;

            // Wait for next frame before continuing loop
            yield return null;
        }

        // Resets the camera back to the original position after the effect
        transform.localPosition = origPos; 
    }
}
