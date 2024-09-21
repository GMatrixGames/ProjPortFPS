using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private GrapplingGun grapplingGun;

    [Header("----- Attributes -----")]
    [SerializeField] [Range(0, 30)] private int hpMax;
    private float hpCurrent;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float airMaxSpeed;
    private float speedCurrent;
    [SerializeField] private float slowdownTimer;
    [SerializeField] [Range(2, 4)] private int sprintMod;
    [SerializeField] [Range(1, 3)] private int jumpMax;
    [SerializeField] [Range(8, 20)] private int jumpSpeed;
    [SerializeField] private int gravity;
    [SerializeField] private CameraShake cameraShake;

    [Header("----- Slide Attributes -----")]
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideHeight = 0.5f;
    [SerializeField] private Transform playerModel;

    private bool isSliding;
    private float originalHeight;

    [Header("----- Sounds -----")]
    [SerializeField] private AudioClip[] audioSteps;
    [SerializeField] [Range(0, 1)] private float audioStepsVolume = 0.5f;

    // Thank you Garrett for teaching me that this region stuff was a thing. This is very nice for decluttering. 

    #region WallRunning

    [Header("----- Wall Running -----")]
    [SerializeField] private int wallRunGravity;
    [SerializeField] private int wallKickMax;
    [SerializeField] private int wallKickSpeed;
    public bool runningOnWall;

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

    #region Grapple Variables

    [SerializeField] float pullThreshold;
    [SerializeField] float pullSpeed;

    #endregion

    #region Weapon

    [Header("----- Weapon -----")]
    [SerializeField] private int headshotMultiplier = 2;
    [SerializeField] private GameObject gunModel;
    [SerializeField] private GameObject muzzleFlash;
    private List<GunStats> gunList = new();
    private float shootRate;
    private int shootDist;
    [SerializeField] private float currentShots;
    private int maxShots;
    private float shootCooldown;
    public bool isCoolingDown;
    public bool hasDropoff;

    #endregion

    private Vector3 move;
    private Vector3 playerVelocity;

    private int jumpCount;
    private float hpOrig;
    private int selectedGun;

    private bool isSprinting;
    private bool isShooting;
    private bool isPlayingStep;

    public int grenadeCount;
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    private Image grenadeIcon;
    private TMP_Text grenadeCountText;
    public GameObject GrenadeOnPlayer;

    private Rigidbody rb;

    #region Damage & Dropoff

    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private float dropOffStart;
    [SerializeField] private float dropOffEnd;

    #endregion

    private float distToGround;

    // Start is called before the first frame update
    private void Start()
    {
        hpOrig = hpMax;
        SpawnPlayer();
        rb = GetComponent<Rigidbody>();
        grenadeCountText = GameManager.instance.grenadeCanvas.GetComponentInChildren<TMP_Text>();
        grenadeIcon = GameManager.instance.grenadeCanvas.GetComponentInChildren<Image>();
        originalHeight = playerModel.localScale.y;
        if (grenadeIcon != null)
        {
            grenadeIcon.enabled = false;
        }

        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    public void SpawnPlayer()
    {
        hpCurrent = hpOrig;
        GameManager.instance.UpdateHealthBar(hpCurrent, hpMax);
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!GameManager.instance.isPaused) // Don't handle movement/shooting if the game is paused.
        {
            Movement();
            SelectGun();

            if (Input.GetKeyDown(SettingsManager.instance.settings.keyBindings["Slide"]) && !isSliding && IsGrounded())
            {
                StartCoroutine(Slide());
            }
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

        if (Input.GetKeyDown(KeyCode.G) && grenadeCount > 0)
        {
            // Debug.Log("G key pressed. Calling ThrowGrenade.");
            ThrowGrenade();
        }

        if (!isShooting)
        {
            currentShots = Mathf.Clamp(currentShots - shootCooldown * Time.deltaTime, 0, 1000);
        }

        if (currentShots == 0)
        {
            isCoolingDown = false;
        }

        if (gunList.Count > 0 && currentShots >= gunList[selectedGun].maxShots)
        {
            isCoolingDown = true;
        }

        speedCurrent = rb.velocity.magnitude;
    }

    /// <summary>
    /// Handling of sprinting mechanic.
    /// </summary>
    private void Movement()
    {
        if (IsGrounded())
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
            lastTouchedWall = null;
        }

        if (grapplingGun.isGrappling)
        {
            PullToPoint();
        }

        var forward = Input.GetKey(SettingsManager.instance.settings.keyBindings["Forward"]) ? 1 : 0;
        var backward = Input.GetKey(SettingsManager.instance.settings.keyBindings["Back"]) ? -1 : 0;
        var left = Input.GetKey(SettingsManager.instance.settings.keyBindings["Left"]) ? -1 : 0;
        var right = Input.GetKey(SettingsManager.instance.settings.keyBindings["Right"]) ? 1 : 0;
        var horizontal = left + right;
        var vertical = forward + backward;

        // Movement logic.
        move = new Vector3(left + right, 0f, forward + backward);
        var moveVector = transform.TransformDirection(move) * accelerationSpeed;

        // Directly set the velocity for more responsive control
        var targetVelocity = moveVector;
        targetVelocity.y = rb.velocity.y; // Preserve the vertical velocity
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * slowdownTimer);

        // Counter movement to stop quickly when no input is given
        if (IsGrounded() && move.magnitude == 0)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * slowdownTimer);
        }

        if (IsGrounded() && horizontal == 0 && vertical == 0)
        {
            //rb.velocity.x to approach 0 quickly. I want this to happen in roughly one sec.
            //How do I do this?
            rb.AddForce(-rb.velocity.x, 0, -rb.velocity.z, ForceMode.Force);
        }

        if (Input.GetKeyDown(SettingsManager.instance.settings.keyBindings["Jump"]) && jumpCount < jumpMax)
        {
            jumpCount++;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        // Checks if you're running on the wall, and if you have a wallkick left. If so, you can kick. 
        if (Input.GetKeyDown(SettingsManager.instance.settings.keyBindings["Jump"]) && runningOnWall && wallKickCount < wallKickMax)
        {
            // Debug.Log($"Wallkick {wallKickCount}");
            wallKickCount++;
            rb.AddForce(Vector3.up * wallKickSpeed, ForceMode.Impulse);
            hasWallKicked = true;
        }

        // Basically checks if you're wallrunning, and you haven't already kicked off this wall. If
        // you haven't, your gravity gets slowed for wallrunning. 
        if (runningOnWall && hasWallKicked == false)
        {
            rb.AddForce(Vector3.down * (wallRunGravity * Time.deltaTime));
        }
        else // Otherwise, use normal gravity
        {
            rb.AddForce(Vector3.down * (gravity * Time.deltaTime));
        }

        if (Input.GetButton("Shoot") && !isShooting && gunList.Count > 0 && !isCoolingDown)
        {
            StartCoroutine(Shoot());
        }

        if (IsGrounded() && move.magnitude > 0.3f && !isPlayingStep && !isSliding)
        {
            StartCoroutine(PlayStep());
        }

        GameManager.instance.UpdateWeaponHeat(currentShots, maxShots);
        speedCurrent = Mathf.Lerp(speedCurrent, accelerationSpeed, slowdownTimer);
    }

    private IEnumerator PlayStep()
    {
        isPlayingStep = true;
        GetComponent<AudioSource>().PlayOneShot(audioSteps[Random.Range(0, audioSteps.Length)], audioStepsVolume);
        yield return new WaitForSeconds(isSprinting ? 0.3f : 0.5f);
        isPlayingStep = false;
    }

    /// <summary>
    /// Handling of sprinting mechanic.
    /// </summary>
    private void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            accelerationSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            accelerationSpeed /= sprintMod;
            isSprinting = false;
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        var cameraOriginalPos = Camera.main.transform.localPosition; // Use cam position so it doesn't squish things attached to it.
        Camera.main.transform.localPosition = new Vector3(cameraOriginalPos.x, cameraOriginalPos.y * slideHeight, cameraOriginalPos.z);
        GetComponent<Collider>().transform.localScale = new Vector3(1, 0.5f, 1); // TODO: FIX THINGS GETTING SQUISHED

        var slideDirection = transform.forward * slideSpeed;
        rb.velocity += new Vector3(slideDirection.x, 0, slideDirection.z);

        yield return new WaitForSeconds(slideDuration);

        GetComponent<Collider>().transform.localScale = Vector3.one; // TODO: FIX THINGS GETTING SQUISHED
        Camera.main.transform.localPosition = new Vector3(cameraOriginalPos.x, cameraOriginalPos.y, cameraOriginalPos.z);
        isSliding = false;
    }

    /// <summary>
    /// Handling of shooting mechanic via raycast.
    /// </summary>
    /// <returns>Time wait based on shootRate</returns>
    private IEnumerator Shoot()
    {
        isShooting = true;
        currentShots++;
        StartCoroutine(FlashMuzzle());

        var shootSounds = gunList[selectedGun].shootSounds ?? Array.Empty<AudioClip>();
        if (shootSounds.Length > 0)
        {
            var randomSound = Random.Range(0, shootSounds.Length - 1);
            gunModel.GetComponent<AudioSource>().PlayOneShot(shootSounds[randomSound], gunList[selectedGun].shootVolume);
        }

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
        if (gunList[selectedGun].hasDropoff)
        {
            if (distance <= dropOffStart) return maxDamage;
            if (distance > dropOffEnd) return 0 /*minDamage*/; // Once drop off end is reached, any bullets past that don't damage. 

            var range = dropOffEnd - dropOffStart;
            var normalizedDistance = (distance - dropOffStart) / range;

            return Mathf.FloorToInt(Mathf.Lerp(maxDamage, minDamage, normalizedDistance));
        }

        return maxDamage;
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

    public void PickUpGrenade(int amount)
    {
        grenadeCount += amount;
        UpdateGrenadeCountDisplay();
        UpdateGrenadeIcon();
    }

    public void GetGunStats(GunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;

        minDamage = gun.minDamage;
        maxDamage = gun.maxDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        maxShots = gun.maxShots;
        shootCooldown = gun.shootCooldown;
        hasDropoff = gun.hasDropoff;

        GameManager.instance.heatBarParent.SetActive(gun.displayHeat);

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        if (gun.gunRotation != default)
        {
            gunModel.transform.localRotation = Quaternion.Euler(gun.gunRotation.x, gun.gunRotation.y, gunModel.transform.localRotation.eulerAngles.z);
            // muzzleFlash.transform.localRotation = gunModel.transform.localRotation;
        }
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
        currentShots = 0;
        var gun = gunList[selectedGun];
        minDamage = gun.minDamage;
        maxDamage = gun.maxDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        maxShots = gun.maxShots;
        shootCooldown = gun.shootCooldown;
        hasDropoff = gun.hasDropoff;

        GameManager.instance.heatBarParent.SetActive(gun.displayHeat);

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        if (gunList[selectedGun].gunRotation != default)
        {
            gunModel.transform.localRotation = Quaternion.Euler(gun.gunRotation.x, gun.gunRotation.y, gunModel.transform.localRotation.eulerAngles.z);
            // muzzleFlash.transform.localRotation = gunModel.transform.localRotation;
        }
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void ThrowGrenade()
    {
        if (grenadePrefab && throwPoint)
        {
            if (grenadeCount > 0)
            {
                var grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);

                var grb = grenade.GetComponent<Rigidbody>();
                if (grb)
                {
                    // Ensure the grenade starts moving in the forward direction
                    var throwDirection = throwPoint.forward;
                    var angle = 45f;
                    var throwSpeed = throwForce;

                    // Calculate the initial velocity
                    var radians = angle * Mathf.Deg2Rad;
                    var horizontalSpeed = throwSpeed * Mathf.Cos(radians);
                    var verticalSpeed = throwSpeed * Mathf.Sin(radians);
                    var initialVelocity = throwDirection * horizontalSpeed;
                    initialVelocity.y = verticalSpeed;

                    grb.velocity = initialVelocity;
                    grb.drag = 0.5f;
                }

                grenade.tag = "Thrown Grenade";

                var grenadeBehaviour = grenade.GetComponent<GrenadeBehaviour>();
                if (grenadeBehaviour)
                {
                    grenadeBehaviour.ActivateExplosion();
                }

                grenadeCount--;
                UpdateGrenadeCountDisplay();
                UpdateGrenadeIcon();

                if (grenadeCount <= 0)
                {
                    GameManager.instance.grenadeCanvas.SetActive(false);

                    if (grenadeIcon)
                    {
                        grenadeIcon.enabled = false; // Hide the grenade icon when the grenade is thrown
                    }

                    if (grenadeCountText)
                    {
                        grenadeCountText.enabled = false; // Hide the grenade count when it reaches zero
                    }
                }
            }
        }
    }

    private void UpdateGrenadeCountDisplay()
    {
        if (grenadeCountText)
        {
            GameManager.instance.grenadeCanvas.SetActive(true);
            grenadeCountText.text = "Grenades: " + grenadeCount; // Update the UI text with the grenade count
        }
    }

    private void UpdateGrenadeIcon()
    {
        GameManager.instance.grenadeCanvas.SetActive(grenadeCount > 0);

        if (grenadeIcon)
        {
            grenadeIcon.enabled = grenadeCount > 0; // Show icon if player has grenades
        }

        if (GrenadeOnPlayer)
        {
            GrenadeOnPlayer.SetActive(grenadeCount > 0); // Show grenade model if player has grenades
        }
    }

    private void UpdateGrenadeUI()
    {
        if (grenadeCountText)
        {
            //Debug.LogError("ThrowPoint / GrenadePrefab not assigned!");
        }
    }

    private void PullToPoint()
    {
        var direction = grapplingGun.grapplePoint - transform.position;

        if (direction.magnitude > pullThreshold)
        {
            direction.Normalize();
            rb.AddForce(direction * (pullSpeed * Time.deltaTime), ForceMode.Impulse);
        }
        else
        {
            grapplingGun.StopGrapple();
        }
    }

    private bool IsGrounded() => Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
}