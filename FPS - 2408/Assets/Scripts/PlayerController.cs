using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask ignoreMask;

    [Header("----- Attributes -----")]
    [SerializeField] [Range(0, 30)] private int hpMax;
    private float hpCurrent;
    [SerializeField] [Range(1, 15)] private int speed;
    [SerializeField] [Range(2, 4)] private int sprintMod;
    [SerializeField] [Range(1, 3)] private int jumpMax;
    [SerializeField] [Range(8, 20)] private int jumpSpeed;
    [SerializeField] [Range(15, 30)] private int gravity;
    [SerializeField] private CameraShake cameraShake;

    // Thank you Garrett for teaching me that this region stuff was a thing. This is very nice for decluttering. 

    #region WallRunning

    [Header("----- Wall Running -----")]
    [SerializeField] private int wallRunGravity;
    [SerializeField] private int wallKickMax;
    [SerializeField] private int wallKickSpeed;
    [SerializeField] private bool runningOnWall;

    private GameObject lastTouchedWall;
    private bool hasWallKicked;
    private int wallKickCount;

    #endregion

    #region HealthRegen

    [Header("----- Health Regeneration -----")]
    [SerializeField] private float healthRegenRate = 1f;
    private bool isTakingDamage;
    private Coroutine regenCoroutine;

    #endregion

    #region Weapon

    [Header("----- Weapon -----")]
    [SerializeField] private int headshotMultiplier = 2;
    [SerializeField] private GameObject gunModel;
    [SerializeField] private GameObject muzzleFlash;
    private List<GunStats> gunList = new();
    private int shootDamage;
    private float shootRate;
    private int shootDist;

    #endregion

    private Vector3 move;
    private Vector3 playerVelocity;

    private int jumpCount;
    private float hpOrig;
    private int selectedGun;

    private bool isSprinting;
    private bool isShooting;

    public bool hasGrenade = false;
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    public GameObject GrenadeOnPlayer;

    #region Damage & Dropoff

    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private float dropOffStart;
    [SerializeField] private float dropOffEnd;

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        hpOrig = hpMax;
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        hpCurrent = hpOrig;
        GameManager.instance.UpdateHealthBar(hpCurrent, hpMax);
        controller.enabled = false; // CharacterController doesn't allow transform to be modified directly, so we disable it temporarily
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!GameManager.instance.isPaused) // Don't handle movement/shooting if the game is paused.
        {
            Movement();
            SelectGun();
        }

        Sprint();
        if (!isTakingDamage)
        {
            // hpCurrent = Mathf.Min(hpCurrent + (int)(healthRegenRate * Time.deltaTime), hpOrig);
            // Debug.Log("Regenerating health: " + healthRegenRate * Time.deltaTime);
            hpCurrent += healthRegenRate * Time.deltaTime;
            if (hpCurrent > hpOrig)
            {
                hpCurrent = hpOrig;
            }

            // Debug.Log("New health: " + hpCurrent);
            GameManager.instance.UpdateHealthBar(hpCurrent, hpMax);
        }

        if (Input.GetKeyDown(KeyCode.G) && hasGrenade)
        {
            Debug.Log("G key pressed. Calling ThrowGrenade.");
            ThrowGrenade();
        }
    }

    /// <summary>
    /// Handling of sprinting mechanic.
    /// </summary>
    private void Movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
            lastTouchedWall = null;
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

        // Checks if you're running on the wall, and if you have a wallkick left. If so, you can kick. 
        if (Input.GetButtonDown("Jump") && runningOnWall && wallKickCount < wallKickMax)
        {
            // Debug.Log($"Wallkick {wallKickCount}");
            wallKickCount++;
            playerVelocity.y = wallKickSpeed;
            hasWallKicked = true;
        }

        controller.Move(playerVelocity * Time.deltaTime);

        // Basically checks if you're wallrunning, and you haven't already kicked off this wall. If
        // you haven't, your gravity gets slowed for wallrunning. 
        if (runningOnWall && hasWallKicked == false)
        {
            playerVelocity.y -= wallRunGravity * Time.deltaTime;
        }
        else // Otherwise, use normal gravity
        {
            playerVelocity.y -= gravity * Time.deltaTime;
        }

        if (Input.GetButton("Shoot") && !isShooting && gunList.Count > 0)
        {
            StartCoroutine(Shoot());
        }
    }

    /// <summary>
    /// Handling of sprinting mechanic.
    /// </summary>
    private void Sprint()
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
    private IEnumerator Shoot()
    {
        isShooting = true;
        StartCoroutine(FlashMuzzle());

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, shootDist, ~ignoreMask))
        {
            var damage = CalcDamage(hit.distance);
            var dmg = hit.collider.GetComponent<IDamage>();

            // Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Head"))
            {
                damage *= headshotMultiplier;
                // Debug.Log("Root Component Name: " + hit.collider.transform.root.name);
                dmg = hit.collider.transform.root.GetComponent<IDamage>();
            }

            // Debug.Log($"Damage @ Distance: {damage} @ {(int) hit.distance}");

            dmg?.TakeDamage(damage);
            Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    private IEnumerator FlashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
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
        if (distance > dropOffEnd) return 0 /*minDamage*/; // Once drop off end is reached, any bullets past that don't damage. 

        var range = dropOffEnd - dropOffStart;
        var normalizedDistance = (distance - dropOffStart) / range;

        return Mathf.FloorToInt(Mathf.Lerp(maxDamage, minDamage, normalizedDistance));
    }

    /// <inheritdoc/>
    public void TakeDamage(int amount)
    {
        hpCurrent -= amount;
        // Debug.Log("Player took damage: " + amount + ", Current HP: " + hpCurrent);
        isTakingDamage = true;

        StartCoroutine(Flash());

        if (hpCurrent <= 0)
        {
            GameManager.instance.StateLost();
            hpCurrent = 0;
            // Debug.Log("Player died.");
        }

        // Trigger camera shake when damaged.
        if (cameraShake && !GameManager.instance.isPaused)
        {
            cameraShake.TriggerShake();
        }

        GameManager.instance.UpdateHealthBar(hpCurrent, hpMax);

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }

        regenCoroutine = StartCoroutine(EnableHealthRegen());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RunnableWall"))
        {
            // Debug.Log("Yep.");
            runningOnWall = true;
            if (other.gameObject != lastTouchedWall)
            {
                hasWallKicked = false;
                wallKickCount = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RunnableWall"))
        {
            // Debug.Log("Nope.");
            runningOnWall = false;
            lastTouchedWall = other.gameObject;
            hasWallKicked = true;
        }
    }

    private static IEnumerator Flash()
    {
        GameManager.instance.damageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.damageFlash.SetActive(false);
    }

    private IEnumerator EnableHealthRegen()
    {
        yield return new WaitForSeconds(5f);
        isTakingDamage = false;
    }

    public void PickUpGrenade()
    {
        hasGrenade = true; // Set the flag to true when the player picks up a grenade
        if (GrenadeOnPlayer != null)
        {
            GrenadeOnPlayer.SetActive(true);
        }
    }

    public void GetGunStats(GunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            ChangeGun();
        }
    }

    private void ChangeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void ThrowGrenade()
    {
        if (grenadePrefab != null && throwPoint != null)
        {
            //Debug.Log("Throw point and grenade prefab are assigned.");

            // Instantiate the grenade at the throw point
            GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
            Debug.Log("Grenade instantiated at position: " + throwPoint.position);


            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Ensure the grenade starts moving in the forward direction
                Vector3 throwDirection = throwPoint.forward;
                float angle = 45f;
                float gravity = Physics.gravity.y;
                float throwSpeed = throwForce;

                // Calculate the initial velocity
                float radians = angle * Mathf.Deg2Rad;
                float horizontalSpeed = throwSpeed * Mathf.Cos(radians);
                float verticalSpeed = throwSpeed * Mathf.Sin(radians);
                Vector3 initialVelocity = throwDirection * horizontalSpeed;
                initialVelocity.y = verticalSpeed;

                
                rb.velocity = initialVelocity;
                rb.drag = 0.5f;
            }
            else
            {
                Debug.LogError("No Rigidbody found on grenade prefab.");
            }
            grenade.tag = "ThrownGrenade"; 
            // Destroy the grenade after some time
            Destroy(grenade, 3f);

            if (GrenadeOnPlayer != null)
            {
                GrenadeOnPlayer.SetActive(false);
            }

            // Reset the flag since the grenade has been thrown
            hasGrenade = false;
        }
        else
        {
            Debug.LogError("ThrowPoint / GrenadePrefab not assigned!");
        }
    }
}