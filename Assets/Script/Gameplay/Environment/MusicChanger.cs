using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // 引入AudioMixer命名空间

public class MusicChanger : MonoBehaviour
{
    public AudioSource bgmSource;                 // 主背景音乐 AudioSource
    public AudioClip newMusicClip;                // 新的背景音乐
    public List<AudioSource> ambientSources;      // 环境音 AudioSource 列表
    public List<AudioClip> ambientClips;          // 环境音效 Clip 列表

    public float fadeDuration = 1f;               // 音乐淡入淡出时长
    public AudioMixer audioMixer;                 // 引用Audio Mixer
    public string bgmVolumeParameter = "BGMVolume";  // Audio Mixer中暴露的背景音乐参数名
    public string ambientVolumeParameter = "AmbientVolume"; // 环境音参数名

    public List<MusicChanger> linkedTriggers;     // 链接触发器列表
    public bool isActive = true;                  // 触发器是否激活

    [Range(0f, 1f)] public float bgmVolume = 1f;      // 背景音乐音量（0-1）
    [Range(0f, 1f)] public float ambientVolume = 1f;  // 环境音音量（0-1）

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            foreach (var trigger in linkedTriggers)
            {
                if (trigger != null) StartCoroutine(ActivateWithDelay(trigger));
            }

            // 淡入新背景音乐
            if (bgmSource != null && newMusicClip != null)
            {
                if (bgmSource.clip != newMusicClip)
                {
                    StartCoroutine(FadeOutAndChangeAudio(bgmSource, newMusicClip, fadeDuration));
                }
            }

            // 调整音量参数
            if (audioMixer != null)
            {
                audioMixer.SetFloat(bgmVolumeParameter, Mathf.Log10(bgmVolume) * 20);
                audioMixer.SetFloat(ambientVolumeParameter, Mathf.Log10(ambientVolume) * 20);
            }
        }
    }

    private IEnumerator ActivateWithDelay(MusicChanger trigger)
    {
        yield return new WaitForSeconds(1f);
        trigger.isActive = true;
    }

    private IEnumerator FadeOutAndChangeAudio(AudioSource audioSource, AudioClip newClip, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        // 渐出
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;

        if (newClip != null)
        {
            audioSource.Play();
        }

        // 渐入
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    private IEnumerator FadeOutAndStopAudio(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        // Fade out
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop(); // Stop the audio after fading out
        audioSource.volume = startVolume; // Reset volume to original for future use
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
        Gizmos.color = isActive ? new Color(1, 0, 0, 0.4f) : new Color(0.5f, 0.5f, 0.5f, 0.5f); // Green if active, gray if inactive

        // Draw the trigger box
        Gizmos.DrawCube(center, size);
    }
}
