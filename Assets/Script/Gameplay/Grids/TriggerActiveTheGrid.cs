using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerActiveTheGrid : MonoBehaviour
{
	public ZeraMovement player;

	public GameObject[] triggerTargets, triggerRenderers;
	public float targetRendererPosY;
	[HideInInspector] public bool canUp = false;

	public bool willChangeTheType;
	public int targetType;
	public WayPoint[] changeTargetWayPoints;

	public bool willPseudoTheTargetPos;
	public WayPoint[] willPseudoWayPoints;

	public bool willChangeTheLayer;
	public GameObject[] changeTargetGridRenderers;
	public string targetLayer;

	[Header("Button Animation and Color Settings")]
	public Transform buttonTransform; // The button's transform for animation
	public Material buttonMaterial; // Material for the button
	public Color pressedColor = Color.green; // Color when the button is pressed
	private Material runtimeMaterial; // Instance material to avoid permanent changes
	public float pressDepth = 0.1f; // How far the button moves when pressed
	public float pressDuration = 0.2f; // How long the press animation lasts

	public AudioSource buttonClickAudioSource; // Audio source for click sound
	public AudioClip buttonClickClip;           // Click sound effect

	public AudioSource movingAudioSource; // Audio source for click sound
	public AudioClip movingClickClip;           // Click sound effect

	[Range(0.5f, 2.0f)] public float audioSpeed = 1.0f; // Slider to adjust audio playback speed
	[Range(0f, 1.0f)] public float audioVolume = 1.0f;  // Slider to adjust audio volume

	void Start()
	{
		// Create a runtime material instance for gameplay
		if (buttonMaterial != null)
		{
			runtimeMaterial = new Material(buttonMaterial); // Clone the material
			if (buttonTransform != null)
				buttonTransform.GetComponent<Renderer>().material = runtimeMaterial; // Apply to button renderer
		}

		// Initialize the audio volume
		if (movingAudioSource != null)
		{
			movingAudioSource.volume = audioVolume;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		ZeraMovement zera = other.GetComponent<ZeraMovement>();
		if (zera != null)
		{
			zera.canClick = false;

			// Play the click sound
			buttonClickAudioSource.PlayOneShot(buttonClickClip);
			// Animate the button press
			AnimateButtonPress();
			// Change the button color
			ChangeButtonColor(pressedColor);

			StartCoroutine(UpTheGridRenderer());
			zera.canClick = true;
		}
	}

	IEnumerator UpTheGridRenderer()
	{
		canUp = true;
		yield return new WaitForSeconds(0.8f);

		// Play the moving sound
		movingAudioSource.pitch = audioSpeed;
		movingAudioSource.PlayOneShot(movingClickClip);

		foreach (GameObject triggerRenderer in triggerRenderers)
		{
			Sequence s = DOTween.Sequence();
			s.Append(triggerRenderer.transform.DOMove(new Vector3(triggerRenderer.transform.position.x, targetRendererPosY, triggerRenderer.transform.position.z), 0.5f).SetEase(Ease.Linear));
			yield return new WaitForSeconds(0.3f);
		}

		foreach (GameObject triggerTarget in triggerTargets)
		{
			triggerTarget.SetActive(true);
		}
		if (willChangeTheType)
		{
			foreach (WayPoint changeTargetWayPoint in changeTargetWayPoints)
			{
				changeTargetWayPoint.type = targetType;
			}
			player.inType = targetType;
		}
		if (willPseudoTheTargetPos)
		{
			foreach (WayPoint willPseudoTarget in willPseudoWayPoints)
			{
				willPseudoTarget.realPos = false;
			}
		}
		if (willChangeTheLayer)
		{
			foreach (GameObject changeTargetGridRenderer in changeTargetGridRenderers)
			{
				changeTargetGridRenderer.layer = LayerMask.NameToLayer(targetLayer);
			}
		}
	}

	void AnimateButtonPress()
	{
		if (buttonTransform == null) return;

		Vector3 originalPosition = buttonTransform.localPosition;
		Vector3 pressedPosition = originalPosition - new Vector3(0, pressDepth, 0); // Move down along the Y-axis

		// Animate the button press
		StartCoroutine(AnimatePosition(buttonTransform, originalPosition, pressedPosition, pressDuration / 2, () =>
		{
			// Animate the button release
			StartCoroutine(AnimatePosition(buttonTransform, pressedPosition, originalPosition, pressDuration / 2, null));
		}));
	}

	IEnumerator AnimatePosition(Transform target, Vector3 start, Vector3 end, float duration, System.Action onComplete)
	{
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			target.localPosition = Vector3.Lerp(start, end, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		target.localPosition = end;

		onComplete?.Invoke();
	}

	void ChangeButtonColor(Color newColor)
	{
		if (runtimeMaterial != null)
		{
			runtimeMaterial.color = newColor;
		}
	}
}
