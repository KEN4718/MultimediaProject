using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum WitchAxis {x,y,z}

public class RotControl : MonoBehaviour
{
	public ChangeAxis changeAxies;

	public ZeraMovement player;
	public float[] destinationAngles;
	public int myAngleIndex;

	private float currentVelocity;

	public float triggerRadius;
	public LayerMask zeraLayer;

	private bool trigger;

	public GameObject bottomRenderer;
	public Color defaultColor, highLightColor;

	public AudioSource audioSource;      // AudioSource to play sounds
	public AudioClip[] soundClips;       // Array of sound clips
	private int currentSoundIndex = 0;   // Index to track the current sound

	void Start()
	{
		//myAngleIndex = 3;
	}

	// Update is called once per frame
	void Update()
	{
		trigger = Physics.CheckSphere(transform.position, triggerRadius, zeraLayer);

		if (player.canClick && trigger)
		{
			bottomRenderer.GetComponent<MeshRenderer>().material.color =
				Color.Lerp(bottomRenderer.GetComponent<MeshRenderer>().material.color, highLightColor, Time.deltaTime * 5f);

			if (Input.GetMouseButtonDown(1))
			{
				PlayNextSound(); // Play the next sound in sequence

				if (myAngleIndex < destinationAngles.Length - 1)
				{
					myAngleIndex += 1;
				}
				else if (myAngleIndex == destinationAngles.Length - 1)
				{
					myAngleIndex = 0;
				}
			}
			//transform.eulerAngles = new Vector3 (0f,destinationAngles[myAngleIndex],0f);
			switch (changeAxies)
			{
				case ChangeAxis.y:
					transform.eulerAngles = new Vector3(0f, Mathf.SmoothDampAngle(transform.eulerAngles.y, destinationAngles[myAngleIndex], ref currentVelocity, 0.3f), 0f);
					if (Mathf.Abs(transform.eulerAngles.y - destinationAngles[myAngleIndex]) <= 5f)
					{
						transform.eulerAngles = new Vector3(0f, destinationAngles[myAngleIndex], 0f);
					}
					break;

				case ChangeAxis.z:
					transform.eulerAngles = new Vector3(0f, 0f, Mathf.SmoothDampAngle(transform.eulerAngles.z, destinationAngles[myAngleIndex], ref currentVelocity, 0.3f));
					if (Mathf.Abs(transform.eulerAngles.z - destinationAngles[myAngleIndex]) <= 5f)
					{
						transform.eulerAngles = new Vector3(0f, 0f, destinationAngles[myAngleIndex]);
					}
					break;
			}

		}
		else
		{
			bottomRenderer.GetComponent<MeshRenderer>().material.color =
				Color.Lerp(bottomRenderer.GetComponent<MeshRenderer>().material.color, defaultColor, Time.deltaTime * 5f);
		}

	}

	void PlayNextSound()
	{
		if (audioSource != null && soundClips.Length > 0)
		{
			// Play the current sound clip
			audioSource.PlayOneShot(soundClips[currentSoundIndex]);

			// Increment the index and loop back to 0 if it exceeds the array length
			currentSoundIndex = (currentSoundIndex + 1) % soundClips.Length;
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, triggerRadius);
	}
}
