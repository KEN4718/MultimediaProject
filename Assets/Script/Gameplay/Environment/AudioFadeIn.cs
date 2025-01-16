using System.Collections;
using UnityEngine;

public class AudioFadeIn : MonoBehaviour
{
    public AudioSource audioSource;  // The audio source to play music
    public float fadeDuration = 2.0f; // Duration of the fade-in effect (in seconds)
    public AudioClip musicClip;      // The music clip to play

    void Start()
    {
        if (audioSource != null && musicClip != null)
        {
            StartCoroutine(FadeInMusic());
        }
    }

    IEnumerator FadeInMusic()
    {
        // Set the audio source to the selected clip and reset volume to 0
        audioSource.clip = musicClip;
        audioSource.volume = 0f;
        audioSource.Play();

        float timeElapsed = 0f;

        // Gradually increase the volume
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
            yield return null;
        }

        // Ensure the volume is set to 1 after the fade-in
        audioSource.volume = 1f;
    }
}
