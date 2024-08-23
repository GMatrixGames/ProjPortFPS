using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Renderer model;

    private Color colorOriginal;

    private void Start()
    {
        colorOriginal = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(FlashModel());
        }
    }

    private IEnumerator FlashModel()
    {
        model.material.color = Color.red;
        GameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.checkpointPopup.SetActive(false);
        model.material.color = colorOriginal;
    }
}