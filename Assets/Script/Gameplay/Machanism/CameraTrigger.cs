using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Vector3 cameraTargetPosition; // Set the desired camera position in the Inspector
    public float cameraTargetSize; // Set the desired camera size in the Inspector

    public Vector3 zoomOutPosition; // Set the zoom-out position in the Inspector
    public float zoomOutSize; // Set the zoom-out size in the Inspector

    public List<CameraTrigger> linkedTriggers; // List of triggers to activate when this one is triggered
    public bool isActive = true; // Whether this trigger is active
    public bool isChat = false; // Whether this trigger is used for a chat event

    private CameraAnimationController cameraController;

    private void Start()
    {
        cameraController = Object.FindFirstObjectByType<CameraAnimationController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" || !isActive) return;

        // Trigger the camera zoom-in
        TriggerCameraZoom();

        if (!isChat)
        {
            // Activate linked triggers with a 3-second delay
            foreach (var trigger in linkedTriggers)
            {
                if (trigger != null) StartCoroutine(ActivateWithDelay(trigger));
            }

            // Only deactivate this trigger if there are linked triggers
            if (linkedTriggers.Count > 0)
            {
                isActive = false; // Deactivate this trigger
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player" || !isChat) return;

        // Trigger the camera zoom-out
        TriggerCameraZoomOut();
    }

    private IEnumerator ActivateWithDelay(CameraTrigger trigger)
    {
        yield return new WaitForSeconds(3f); // Delay for 3 seconds
        trigger.isActive = true; // Activate the trigger
    }

    private void TriggerCameraZoom()
    {
        cameraController.ZoomTo(cameraTargetPosition, cameraTargetSize);
    }

    private void TriggerCameraZoomOut()
    {
        cameraController.ZoomTo(zoomOutPosition, zoomOutSize);
    }

    private void OnDrawGizmos()
    {
        // Access the BoxCollider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null) return; // Exit if no BoxCollider is attached

        // Calculate the collider's center and size in world space
        Vector3 center = transform.position + transform.rotation * boxCollider.center;
        Vector3 size = boxCollider.size;

        // Set Gizmo color based on activation state
        Gizmos.color = isActive ? new Color(0, 1, 0, 0.4f) : new Color(0.5f, 0.5f, 0.5f, 0.5f); // Green if active, gray if inactive

        // Draw the trigger box
        Gizmos.DrawCube(center, size);

        // Draw linked triggers
        Gizmos.color = new Color(1, 1, 0, 0.6f); // Yellow lines for linked triggers
        foreach (var trigger in linkedTriggers)
        {
            if (trigger != null)
            {
                Gizmos.DrawLine(center, trigger.transform.position);
            }
        }
    }
}
