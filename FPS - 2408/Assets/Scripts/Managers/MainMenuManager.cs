using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        AudioListener.volume = SettingsManager.instance.settings.GetVolume();
    }
}