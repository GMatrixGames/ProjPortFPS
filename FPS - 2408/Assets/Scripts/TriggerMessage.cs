using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    [SerializeField] GameObject messageToDisplay;
    [SerializeField] float timeToDisplay;

    private bool hasTriggered = false;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !hasTriggered)
        {
            StartCoroutine(ShowMessage());
        }
    }

    IEnumerator ShowMessage()
    {
        hasTriggered = true;
        messageToDisplay.SetActive(true);
        yield return new WaitForSeconds(timeToDisplay);
        messageToDisplay.SetActive(false);
    }
}
