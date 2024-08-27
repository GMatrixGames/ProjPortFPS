using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject menuActive;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;

    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text spawnersCountText;
    [SerializeField] private TMP_Text healthText;

    #region Player

    public GameObject playerSpawnPos { get; private set; }
    public GameObject player { get; private set; }
    public PlayerController playerScript { get; private set; }
    public Image healthBar;
    public Image fuelBar;
    public GameObject damageFlash;

    #endregion

    public GameObject checkpointPopup;

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

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (!menuActive)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }

        if (killCount >= totalEnemies && spawnersDestroyedCount >= spawnersCount)
        {
            if (menuActive != menuWin) // Since we're handling this in Update instead of in the goal updates, we need to check if the menu is already active.
            {
                StatePause();
                menuActive = menuWin;
                menuActive.SetActive(isPaused);
            }
        }
    }

    /// <summary>
    /// Set the state of the game to "Paused"
    /// </summary>
    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // GK: Personally dislike exclusive windowed, so it's None.
    }

    /// <summary>
    /// Set the state of the game to "Unpaused"
    /// </summary>
    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScale; // GK: Utilize stored timescale.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
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

    /// <summary>
    /// Update the Fuel bar fill amount.
    /// </summary>
    ///<param name="fuelCurr"> Players current Fuel</param>
    ///<param name="fuelMax"> Players max Fuel</param>
    public void UpdateFuelBar(float fuelCurr, int fuelMax)
    {
        Debug.Log($"Updating Fuel Bar: Current Fuel = {fuelCurr}/{fuelMax}");
        if (fuelBar)
        {
            fuelBar.fillAmount = fuelCurr / fuelMax;
            Debug.Log("Fuel Bar Fill Amount: " + fuelBar.fillAmount);
        }
        else
        {
            Debug.LogError("Fuel Bar reference is missing!");
        }
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
        menuActive.SetActive(isPaused);
    }
}