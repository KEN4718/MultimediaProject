using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAndDisactiveTheGrid : MonoBehaviour
{
	public GameObject[] activeGrids, disActiveGrids;
	public bool canFallDown = true;

	public AudioSource soundAudioSource; // Audio source for sound
	public AudioClip soundClip;           // Sound effect

	[Range(0.5f, 2.0f)] public float audioSpeed = 1.0f; // Slider to adjust audio playback speed
	[Range(0f, 1.0f)] public float audioVolume = 1.0f;  // Slider to adjust audio volume

	// Use this for initialization
	void Start()
	{
		// Initialize the audio volume
		if (soundAudioSource != null)
		{
			soundAudioSource.volume = audioVolume;
		}
	}


	IEnumerator OnTriggerEnter(Collider other)
	{
		ZeraMovement zera = other.GetComponent<ZeraMovement>();
		if (zera != null)
		{
			// Play the moving sound
			soundAudioSource.pitch = audioSpeed;
			soundAudioSource.PlayOneShot(soundClip);

			zera.canClick = false;
			foreach (GameObject activeGrid in activeGrids)
			{
				activeGrid.SetActive(true);
			}
			if (canFallDown)
			{
				foreach (GameObject disactiveGrid in disActiveGrids)
				{
					yield return new WaitForSeconds(0.2f);
					if (disactiveGrid.GetComponent<Rigidbody>() == null)
					{
						disactiveGrid.AddComponent<Rigidbody>();
					}

				}
			}

			yield return new WaitForSeconds(0.8f);
			foreach (GameObject disactiveGrid in disActiveGrids)
			{
				disactiveGrid.SetActive(false);
			}
			yield return new WaitForSeconds(0.2f);
			zera.canClick = true;
		}
	}
}
