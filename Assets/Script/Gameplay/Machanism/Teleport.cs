using UnityEngine;
using System.Collections;

public class Teleportation : MonoBehaviour
{
    public float delayTime = 2f;
    public Transform targetBlock;
    public float angle;
    private bool isInArea = false;

    private void OnTriggerStay(Collider other)
    {
        ZeraMovement zera = other.GetComponent<ZeraMovement>();
        if (other.CompareTag("Player") && !isInArea)
        {
            isInArea = true;
            StartCoroutine(TeleportAfterDelay(other.gameObject, zera));
        }
    }

    private IEnumerator TeleportAfterDelay(GameObject player, ZeraMovement zera)
    {
        yield return new WaitForSeconds(delayTime);

        // Unparent player
        player.transform.parent = null;

        // Update position and rotation
        Vector3 targetPosition = new Vector3(targetBlock.position.x, targetBlock.position.y + 1.5f, targetBlock.position.z);
        player.transform.position = targetPosition;
        player.transform.rotation = Quaternion.Euler(0, angle, 0);

        // Reset states
        zera.CheckWhichGridIn();
        player.transform.parent = targetBlock;
        zera.curretVelocity = Vector3.zero;
        zera.canClick = true;

        isInArea = false;
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null) return;
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.5f); // Light blue with 50% transparency
        Vector3 center = transform.position + transform.rotation * boxCollider.center;
        Vector3 size = boxCollider.size;
        Gizmos.DrawCube(center, size);

        if (targetBlock != null)
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawCube(targetBlock.position, new Vector3(1.2f, 1.2f, 1.2f));
        }
    }
}


