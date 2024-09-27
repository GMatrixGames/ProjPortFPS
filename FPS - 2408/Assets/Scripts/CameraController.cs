using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private int sens;
    [SerializeField] private int lockVertMin, lockVertMax;
    [SerializeField] private bool invertY;

    private float rotX;
    private bool isAlreadyTilted;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // input
        var mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        var mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        if (invertY) rotX += mouseY;
        else rotX -= mouseY;

        // clamp x rotation on x
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate camera on x
        var movementRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate the player on y
        // GK: Why was this so confusing/annoying to figure out???
        // Why does the rigidbody only SOMETIMES fight the transform rotation???
        var rb = GetComponentInParent<Rigidbody>();
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, mouseX, 0));
        // transform.parent.Rotate(Vector3.up * mouseX);

        // Wall Running feedback is not at a point where we want it at this stage, so it did not make it into the Beta milestone. I apologize.
        // if (playerController.runningOnWall && !isAlreadyTilted)
        // {
        //     var targetTilt = playerController.isLeaningRight ? -15 : 15;
        //     var targetTiltRotation = Quaternion.Euler(0, 0, targetTilt);
        //
        //     // Smoothly tilt the camera towards the target tilt rotation
        //     transform.localRotation = Quaternion.Slerp(transform.localRotation, movementRotation * targetTiltRotation, Time.deltaTime * 5f);
        //
        //     // Check if the tilt is nearly done
        //     if (Quaternion.Angle(transform.localRotation, movementRotation * targetTiltRotation) < 0.1f)
        //     {
        //         isAlreadyTilted = true;
        //     }
        // }
        // else if (!playerController.runningOnWall && isAlreadyTilted)
        // {
        //     // Smoothly return to the original rotation (no tilt, just movement)
        //     transform.localRotation = Quaternion.Slerp(transform.localRotation, movementRotation * originalRotation, Time.deltaTime * 5f);
        //
        //     // Check if the rotation is back to normal
        //     if (Quaternion.Angle(transform.localRotation, movementRotation * originalRotation) < 0.1f)
        //     {
        //         isAlreadyTilted = false;
        //     }
        // }
        // else
        // {
            // Apply camera movement when not tilting
            transform.localRotation = movementRotation;
        // }
    }
}