using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClickMoveTheGrids : MonoBehaviour
{
	public ZeraMovement player;
	public ChangeAxis changeAxis;
	public float[] targetPoses;
	public Transform[] myOrganRenderers;
	public WayPoint mywayPoint;
	private int posIndex;
	public Transform myTransform;

	public Color defaultColor, highLightColor;

	public AudioSource soundAudioSource; // Audio source for sound
	public AudioClip soundClip;           // Sound effect

	[Range(0.5f, 4.0f)] public float audioSpeed = 1.0f; // Slider to adjust audio playback speed
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

	// Update is called once per frame
	void Update()
	{
	
		if (player.canClick)
		{
			foreach (Transform myOrganRenderer in myOrganRenderers)
			{
				myOrganRenderer.GetComponent<MeshRenderer>().material.color =
					Color.Lerp(myOrganRenderer.GetComponent<MeshRenderer>().material.color, highLightColor, Time.deltaTime * 5f);
			}

		}
		else
		{
			foreach (Transform myOrganRenderer in myOrganRenderers)
			{
				myOrganRenderer.GetComponent<MeshRenderer>().material.color =
					Color.Lerp(myOrganRenderer.GetComponent<MeshRenderer>().material.color, defaultColor, Time.deltaTime * 5f);
			}
		}

		if (Input.GetMouseButtonDown(1) && player.canClick)
		{
			Sequence s = DOTween.Sequence();
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit mouseHit;
			if (Physics.Raycast(mouseRay, out mouseHit))
			{
				if (mouseHit.transform.GetComponent<ClickMoveTheGrids>() != null && mouseHit.collider.gameObject == this.gameObject)
				{
					// Play the moving sound
					soundAudioSource.pitch = audioSpeed;
					soundAudioSource.PlayOneShot(soundClip);

					if (posIndex < targetPoses.Length - 1)
					{
						posIndex += 1;
					}
					else if (posIndex == targetPoses.Length - 1)
					{
						posIndex = 0;
					}


					switch (changeAxis)
					{
						case ChangeAxis.x:
							//myTransform.position = new Vector3 (targetPoses[posIndex],myTransform.position.y,myTransform.position.z);
							s.Append(myTransform.transform.DOMove(new Vector3(targetPoses[posIndex], myTransform.position.y, myTransform.position.z), 0.2f).SetEase(Ease.Linear));
							break;
						case ChangeAxis.y:
							//myTransform.position = new Vector3 (targetPoses[posIndex],myTransform.position.y,myTransform.position.z);
							s.Append(myTransform.transform.DOMove(new Vector3(myTransform.position.x, targetPoses[posIndex], myTransform.position.z), 0.2f).SetEase(Ease.Linear));
							break;
					}
				}
			}
		}

		if (changeAxis == ChangeAxis.y)
		{
			if (Mathf.Abs(player.transform.position.y - mywayPoint.transform.position.y) < 1.1f)
			{
				mywayPoint.gameObject.SetActive(true);
			}
			else
			{
				mywayPoint.gameObject.SetActive(false);
			}
		}
	}
}
