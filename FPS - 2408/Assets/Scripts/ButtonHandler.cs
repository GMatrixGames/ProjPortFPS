using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    /// <summary>
    /// Resume game from pause menu.
    /// </summary>
    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }

    /// <summary>
    /// Restart the level
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}