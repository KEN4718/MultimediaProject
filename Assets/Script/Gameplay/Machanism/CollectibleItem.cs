using UnityEngine;
using System.Collections;

public class CollectibleItem : MonoBehaviour
{
    public GameObject[] blocksToActivate;   // Array of blocks to activate
    public GameObject[] blocksToDeactivate; // Array of blocks to deactivate
    public int collectibleID;               // Unique ID for each collectible item
    public float destroyDelay = 1f;         // Delay time before destruction, in seconds
    public bool isTriggered = false; // 用于记录是否已触发
    public float delayTime = 1f;

    public GlowingTransition glowingTransition; // Reference to the GlowingTransition script

    public AudioSource glowingAudioSource; // Audio source for footstep sounds
    public AudioClip glowingClip;           // Footstep sound effect
    public TriggerDialogue triggerDialogue; // Reference to TriggerDialogue script


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player triggered the collider
        {
            // Trigger the glowing transition at the player's position
            if (glowingTransition != null)
            {
                glowingAudioSource.PlayOneShot(glowingClip);
                glowingTransition.StartGlowingTransition(other.transform.position, 3f);
            }
            isTriggered = true; // 标记为触发
            StartCoroutine(DelayedActions()); // Start the coroutine for delayed destruction

            // 触发 ReplaceCharacter1Image 方法来更换图片
            if (triggerDialogue != null)
            {
                triggerDialogue.ReplaceCharacter1Image(); // 在此时调用更新图片
            
                triggerDialogue.UpdateFirstTimeDialogues();
            }
        }
    }

    private IEnumerator DelayedActions()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delayTime);

        // Perform the remaining actions
        ActivateBlocks();   // Activate the blocks
        DeactivateBlocks(); // Deactivate the blocks
        DestroyAfterDelay(); // Start the coroutine for delayed destruction
    }

    private void ActivateBlocks()
    {
        foreach (GameObject block in blocksToActivate)
        {
            block.SetActive(true); // Activate each block
        }
    }

    private void DeactivateBlocks()
    {
        foreach (GameObject block in blocksToDeactivate)
        {
            block.SetActive(false); // Deactivate each block
        }
    }

    private void DestroyAfterDelay()
    {
        // Destroy the collectible item after the delay
        Destroy(gameObject);
    }
}
