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

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartGrapple();
            shouldDrawRope = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
            shouldDrawRope = false;
        }
    }

    private void LateUpdate()
    {
        if (shouldDrawRope)
        {
            DrawLine();
        }
    }

    public void StartGrapple()
    {
        grapplePoint = transform.position;
        RaycastHit hit;

        if(Physics.Raycast(cameraPoint.position, cameraPoint.forward, out hit, maxDistance, grappleLayer))
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
        lineRenderer.SetPosition(0, grappleTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
