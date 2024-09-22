using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private int sens;
    [SerializeField] private int lockVertMin, lockVertMax;
    [SerializeField] private bool invertY;

    private PlayerController playerController;
    private float rotX;
    private Quaternion originalRotation;
    private bool isAlreadyTilted;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController = GameManager.instance.playerScript;
        originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        // input
        // The camera bugginess is happening here, but I've got no idea how to resolve it without using fixed update and fixedDeltaTime and making it not as smooth.
        var mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        var mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        if (invertY) rotX += mouseY;
        else rotX -= mouseY;

        // clamp x rotation on x
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate camera on x
        var movementRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate the player on y
        transform.parent.Rotate(Vector3.up * mouseX);

        if (playerController.runningOnWall && !isAlreadyTilted)
        {
            var targetTilt = playerController.isLeaningRight ? -15 : 15;
            var targetTiltRotation = Quaternion.Euler(0, 0, targetTilt);

            // Smoothly tilt the camera towards the target tilt rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, movementRotation * targetTiltRotation, Time.deltaTime * 5f);
        
            // Check if the tilt is nearly done
            if (Quaternion.Angle(transform.localRotation, movementRotation * targetTiltRotation) < 0.1f)
            {
                isAlreadyTilted = true;
            }
        }
        else if (!playerController.runningOnWall && isAlreadyTilted)
        {
            // Smoothly return to the original rotation (no tilt, just movement)
            transform.localRotation = Quaternion.Slerp(transform.localRotation, movementRotation * originalRotation, Time.deltaTime * 5f);
        
            // Check if the rotation is back to normal
            if (Quaternion.Angle(transform.localRotation, movementRotation * originalRotation) < 0.1f)
            {
                isAlreadyTilted = false;
            }
        }
        else
        {
            // Apply camera movement when not tilting
            transform.localRotation = movementRotation;
        }
    }
}