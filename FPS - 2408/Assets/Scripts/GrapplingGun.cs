using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] LayerMask grappleLayer;
    public Vector3 grapplePoint;
    [SerializeField] Transform grappleTip, cameraPoint;
    [SerializeField] float maxDistance = 50f;

    bool shouldDrawRope;
    public bool isGrappling;

    RaycastHit hit;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !GameManager.instance.grappleShouldCooldown)
        {
            StartGrapple();
            if (isGrappling)
            {
                shouldDrawRope = true;
                GameManager.instance.UpdateGrappleCD();
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
            shouldDrawRope = false;
        }
    }

    private void LateUpdate()
    {
        if (shouldDrawRope && isGrappling)
        {
            DrawLine();
        }
    }

    public void StartGrapple()
    {
        grapplePoint = transform.position;

        if (Physics.Raycast(cameraPoint.position, cameraPoint.forward, out hit, maxDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;
        }

        lineRenderer.positionCount = 2;
    }

    public void StopGrapple()
    {
        lineRenderer.positionCount = 0;
        isGrappling = false;
    }

    void DrawLine()
    {
        //THis is nothing, so that I can recommit. 
        lineRenderer.SetPosition(0, grappleTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
