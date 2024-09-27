using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject menuActive;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;
    [SerializeField] private GameObject menuOptions;

    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text spawnersCountText;
    [SerializeField] private TMP_Text healthText;

    #region Player

    public GameObject playerSpawnPos { get; private set; }
    public GameObject player { get; private set; }
    public PlayerController playerScript { get; private set; }
    public Image healthBar;
    public Image heatBar;
    public Image grappleCooldownImage;
    [SerializeField] private TMP_Text grappleCooldownText;
    public GameObject damageFlash;
    public GameObject heatBarParent;

    #endregion

    #region GrappleCD

    public bool grappleShouldCooldown;
    [SerializeField] float grappleCooldownTime;
    private float grappleCooldownTimer = 0f;

    #endregion

    public GameObject checkpointPopup;
    public GameObject grenadeCanvas;

    public bool isPaused;

    private int killCount;
    private int totalEnemies;
    private int spawnersDestroyedCount;
    private int spawnersCount;

    // GK: Custom timeScale, should be 1 by default.
    [SerializeField] private int timeScale = 1;

    // Awake allows for initialization before other game objects
    private void Awake()
    {
        instance = this;
        Time.timeScale = timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }

    private void Start()
    {
        grappleCooldownImage.fillAmount = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(SettingsManager.instance.settings.keyBindings["Pause"]))
        {
            if (!menuActive)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.transform.Find("Quit").gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }

        if (killCount >= totalEnemies && spawnersDestroyedCount >= spawnersCount)
        {
            if (menuActive != menuWin) // Since we're handling this in Update instead of in the goal updates,
                // we need to check if the menu is already active.
            {
                StatePause();
                menuActive = menuWin;
                menuActive.transform.Find("Quit").gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
                menuActive.SetActive(isPaused);
            }
        }

        if (grappleShouldCooldown)
        {
            GrappleCooldown();
        }
    }

    /// <summary>
    /// Set the state of the game to "Paused"
    /// </summary>
    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // GK: Personally dislike exclusive windowed, so it's None.
    }

    /// <summary>
    /// Set the state of the game to "Unpaused"
    /// </summary>
    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScale; // GK: Utilize stored timescale.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void OpenPauseMenu()
    {
        if (menuActive) menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.transform.Find("Quit").gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
        menuActive.SetActive(true);
    }

    /// <summary>
    /// Open the options menu
    /// </summary>
    public void OpenOptionsMenu()
    {
        if (menuActive) menuActive.SetActive(false);
        menuActive = menuOptions;
        menuActive.SetActive(true);
    }

    public void UpdateWeaponHeat(float currShots, float maxShots)
    {
        if (heatBar)
        {
            heatBar.fillAmount = currShots / maxShots;
        }
        else
        {
            Debug.LogError("Ayo you missin' shit.");
        }
    }

    /// <summary>
    /// Update the HP bar fill amount.
    /// </summary>
    ///<param name="hpCurr"> Players current HP</param>
    ///<param name="hpMax"> Players max HP</param>
    public void UpdateHealthBar(float hpCurr, int hpMax)
    {
        // Debug.Log($"Updating Health Bar: Current HP = {hpCurr}/{hpMax}");
        if (healthBar)
        {
            healthBar.fillAmount = hpCurr / hpMax;
            healthText.text = $"{(int) hpCurr} / {hpMax}";
            // Debug.Log("Health Bar Fill Amount: " + healthBar.fillAmount);
        }
        else
        {
            Debug.LogError("Health Bar reference is missing!");
        }
    }

    void GrappleCooldown()
    {
        grappleCooldownTimer -= Time.deltaTime;

        if (grappleCooldownTimer < 0f)
        {
            grappleShouldCooldown = false;
            grappleCooldownText.gameObject.SetActive(false);
            grappleCooldownImage.fillAmount = 0f;
        }
        else
        {
            grappleCooldownText.text = grappleCooldownTimer.ToString("F1");
            grappleCooldownImage.fillAmount = grappleCooldownTimer / grappleCooldownTime;
        }
    }

    public void UpdateGrappleCD()
    {
        if (grappleShouldCooldown) return;
        grappleShouldCooldown = true;
        grappleCooldownText.gameObject.SetActive(true);
        grappleCooldownTimer = grappleCooldownTime;
    }

    /// <summary>
    /// Update the goal amount.
    /// </summary>
    /// <param name="amount">Amount of enemies killed</param>
    public void UpdateEnemyGoal(int amount)
    {
        killCount += amount;
        killCountText.text = $"{killCount:D2}/{totalEnemies:D2}";
    }

    public void UpdateEnemyMax(int amount)
    {
        totalEnemies += amount;
        killCountText.text = $"{killCount:D2}/{totalEnemies:D2}";
    }

    public void UpdateSpawnersGoal(int amount)
    {
        spawnersDestroyedCount += amount;
        spawnersCountText.text = $"{spawnersDestroyedCount:D2}/{spawnersCount:D2}";
    }

    public void UpdateSpawnersMax(int amount)
    {
        spawnersCount += amount;
        spawnersCountText.text = $"{spawnersDestroyedCount:D2}/{spawnersCount:D2}";
    }
 
    /// <summary>
    /// Set state to lost
    /// </summary>
    public void StateLost()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.transform.Find("Quit").gameObject.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
        menuActive.SetActive(isPaused);
    }
}