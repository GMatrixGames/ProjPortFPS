using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private int hp;
    [SerializeField] private int speed;
    [SerializeField] private int sprintMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpSpeed;
    [SerializeField] private int gravity;
    [SerializeField] private int wallKickMax;
    [SerializeField] private int wallKickSpeed;

    [SerializeField] private bool runningOnWall;

    [SerializeField] private float shootRate;
    [SerializeField] private int shootDist;

    private Vector3 move;
    private Vector3 playerVelocity;

    private int jumpCount;
    private int wallKickCount;
    private int hpOrig;

    private bool isSprinting;
    private bool isShooting;
    private GameObject lastTouchedWall;

    #region Damage & Dropoff

    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private float dropOffStart;
    [SerializeField] private float dropOffEnd;

    private Vector3 startPos;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused) // Don't handle movement/shooting if the game is paused.
        {
            Movement();
        }

        Sprint();
    }

    /// <summary>
    /// Handling of sprinting mechanic.
    /// </summary>
    void Movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

        // move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // transform.position += move * speed * Time.deltaTime;

        move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        controller.Move(move * (speed * Time.deltaTime));

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }

        if (Input.GetButtonDown("Jump") && runningOnWall == true && wallKickCount < wallKickMax)
        {
            wallKickCount++;
            playerVelocity.y = wallKickSpeed;
        }

        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    /// <summary>
    /// Handling of sprinting mechanic.
    /// </summary>
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    /// <summary>
    /// Handling of shooting mechanic via raycast.
    /// </summary>
    /// <returns>Time wait based on shootRate</returns>
    IEnumerator Shoot()
    {
        isShooting = true;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, shootDist, ~ignoreMask))
        {
            var damage = CalcDamage(hit.distance);

            Debug.Log($"Damage @ Distance: {damage} @ {(int) hit.distance}");

            var dmg = hit.collider.GetComponent<IDamage>();
            dmg?.TakeDamage(damage);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    /// <summary>
    /// Calculate the damage based on the distance traveled by the bullet.
    /// If distance is less than the dropoff start, return max damage.
    /// If distance is greater than the dropoff end, return minimum damage.
    /// </summary>
    /// <param name="distance">Distance the bullet has traveled</param>
    /// <returns>calculated damage</returns>
    private int CalcDamage(float distance)
    {
        if (distance <= dropOffStart) return maxDamage;
        if (distance >= dropOffEnd) return minDamage;

        var range = dropOffEnd - dropOffStart;
        var normalizedDistance = (distance - dropOffStart) / range;

        return Mathf.FloorToInt(Mathf.Lerp(maxDamage, minDamage, normalizedDistance));
    }

    /// <inheritdoc/>
    public void TakeDamage(int amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            GameManager.instance.StateLost();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Debug.Log("Yep.");
            runningOnWall = true;
            if (other.gameObject != lastTouchedWall)
            {
                wallKickCount = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Debug.Log("Nope.");
            runningOnWall = false;
            lastTouchedWall = other.gameObject;
        }
    }
}