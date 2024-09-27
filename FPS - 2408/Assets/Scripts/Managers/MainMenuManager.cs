using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject quitButton;

    private void Start()
    {
        AudioListener.volume = SettingsManager.instance.settings.GetVolume();

        quitButton.SetActive(Application.platform != RuntimePlatform.WebGLPlayer);
    }

    private void Update()
    {
        // Rotate the skybox on main menu
        RenderSettings.skybox.SetFloat("_Rotation", Time.time);
    }
}