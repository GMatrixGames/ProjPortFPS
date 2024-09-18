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
}