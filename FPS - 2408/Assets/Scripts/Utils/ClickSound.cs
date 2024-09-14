using UnityEngine;

public class ClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlayClickSound()
    {
        audioSource.time = .2f;
        audioSource.Play();
    }
}