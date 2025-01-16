using UnityEngine;

public class ShowInteractableUI : MonoBehaviour
{
    public GameObject eButtonUI; // Reference to the UI element (e.g., "E" button or text)

    private bool isPlayerInRange = false;

    private void Start()
    {
        // Ensure the UI starts as hidden
        if (eButtonUI != null)
        {
            eButtonUI.SetActive(false);
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assumes the player has a tag "Player"
        {
            isPlayerInRange = true;
            if (eButtonUI != null)
            {
                eButtonUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (eButtonUI != null)
            {
                eButtonUI.SetActive(false);
            }
        }
    }
}
