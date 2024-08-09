using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject menuActive;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;

    #region Player

    public GameObject player { get; private set; }
    public PlayerController playerScript { get; private set; }

    #endregion

    public bool isPaused;

    private int enemyCount;

    // GK: Custom timeScale, should be 1 by default.
    [SerializeField] private int timeScale = 1;

    // Awake allows for initialization before other game objects
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
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
    /// Update the goal amount.
    /// </summary>
    /// <param name="amount">Amount of enemies killed</param>
    public void UpdateGoal(int amount)
    {
        enemyCount += amount;

        if (enemyCount <= 0)
        {
            StateUnpause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);
        }
    }
}