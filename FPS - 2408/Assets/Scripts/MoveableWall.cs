using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableWall : MonoBehaviour
{
    [SerializeField] private List<GameObject> connectedSpawners;
    [SerializeField] private float wallMoveSpeed;
    [SerializeField] private float zDestination;

    private bool hasMoved = false;

    // Update is called once per frame
    void Update()
    {
        connectedSpawners.RemoveAll(gameObject => gameObject == null);

        if (connectedSpawners.Count == 0 && hasMoved == false)
        {
            StartCoroutine(MoveWall());
        }

        if (transform.position.z >= zDestination)
        {
            StopCoroutine(MoveWall()); 
            hasMoved = true;    
        }
    }

    IEnumerator MoveWall()
    {
        transform.position += new Vector3(0, 0 ,wallMoveSpeed) * Time.deltaTime;

        yield return new WaitForSeconds(0.1f);
    }
}
