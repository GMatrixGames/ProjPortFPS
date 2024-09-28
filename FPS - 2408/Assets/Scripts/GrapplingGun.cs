using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] LayerMask grappleLayer;
    public Vector3 grapplePoint;
    [SerializeField] Transform grappleTip, cameraPoint;
    [SerializeField] float maxDistance = 50f;
    [SerializeField] private AudioClip gunAudio;

    bool shouldDrawRope;
    public bool isGrappling;

    RaycastHit hit;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        var grappleKey = SettingsManager.instance.settings.keyBindings["Grapple"];

        if (Input.GetKeyDown(grappleKey) && !GameManager.instance.grappleShouldCooldown && Time.timeScale != 0)
        {
            StartGrapple();
            if (isGrappling)
            {
                shouldDrawRope = true;
                GameManager.instance.UpdateGrappleCD();
            }
        }
        else if (Input.GetKeyUp(grappleKey))
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

            GameManager.instance.player.GetComponent<AudioSource>().PlayOneShot(gunAudio, .5f);
        }

        lineRenderer.positionCount = 2;
    }

    public void StopGrapple()
    {
        lineRenderer.positionCount = 0;
        isGrappling = false;
    }

    private void DrawLine()
    {
        //THis is nothing, so that I can recommit. 
        lineRenderer.SetPosition(0, grappleTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
