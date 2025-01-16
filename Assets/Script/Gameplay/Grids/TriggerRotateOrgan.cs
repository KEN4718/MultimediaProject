using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChangeAxis { x, y, z }

public class TriggerRotateOrgan : MonoBehaviour
{
	public ChangeAxis changeAxies;
	public Transform targetRotObject;
	public ZeraMovement zera;

	public float targetAngle;
	private float currentVelocity;
	private bool canRot = false;

	[Header("Button Animation Settings")]
	public Transform buttonTransform; // Assign the button's Transform
	public Material buttonMaterial; // Assign the button's material
	private Material runtimeMaterial;
	public Color pressedColor = Color.green; // Color to change to when triggered
	public float pressDepth = 0.1f; // How far the button moves when pressed
	public float pressDuration = 0.2f; // How long the press animation lasts

	public AudioSource ButtonClickAudioSource; // Audio source for click sound
	public AudioClip buttonClickClip;           // Click sound effect

	void Start()
	{
		// Create a unique material instance for this object at runtime
		if (buttonMaterial != null)
		{
			runtimeMaterial = new Material(buttonMaterial); // Clone the material
			buttonTransform.GetComponent<Renderer>().material = runtimeMaterial; // Assign the cloned material
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (canRot)
		{
			RotateTheTarget();
		}

		switch (changeAxies)
		{
			case ChangeAxis.x:
				if (Mathf.Abs(targetRotObject.eulerAngles.x - targetAngle) < 5f)
				{
					targetRotObject.eulerAngles = new Vector3(targetAngle, 0, 0);
				}
				break;
			case ChangeAxis.y:
				if (Mathf.Abs(targetRotObject.eulerAngles.y - targetAngle) < 5f)
				{
					targetRotObject.eulerAngles = new Vector3(0, targetAngle, 0);
				}
				break;
			case ChangeAxis.z:
				if (Mathf.Abs(targetRotObject.eulerAngles.z - targetAngle) < 5f)
				{
					targetRotObject.eulerAngles = new Vector3(0, 0, targetAngle);
				}
				break;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		ZeraMovement zera = other.GetComponent<ZeraMovement>();
		if (zera != null)
		{
			ChangeButtonColor(pressedColor);
			ButtonClickAudioSource.PlayOneShot(buttonClickClip);
			AnimateButtonPress();
			Invoke("TrueRot", 0.5f);
		}
	}

	void TrueRot()
	{
		canRot = true;
	}

	void RotateTheTarget()
	{
		switch (changeAxies)
		{
			case ChangeAxis.x:
				targetRotObject.eulerAngles = new Vector3(Mathf.SmoothDampAngle(targetRotObject.eulerAngles.x, targetAngle, ref currentVelocity, 0.15f), 0, 0);
				break;
			case ChangeAxis.y:
				targetRotObject.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(targetRotObject.eulerAngles.y, targetAngle, ref currentVelocity, 1f), 0);
				break;
			case ChangeAxis.z:
				targetRotObject.eulerAngles = new Vector3(0, 0, Mathf.SmoothDampAngle(targetRotObject.eulerAngles.z, targetAngle, ref currentVelocity, 0.15f));
				break;
		}
		//targetRotObject.eulerAngles = new Vector3 (90, 0, 0);
	}

	void AnimateButtonPress()
	{
		if (buttonTransform == null) return;

		Vector3 originalPosition = buttonTransform.localPosition;
		Vector3 pressedPosition = originalPosition - new Vector3(0, pressDepth, 0); // Move down along the Y-axis

		// Move the button down
		StartCoroutine(AnimatePosition(buttonTransform, originalPosition, pressedPosition, pressDuration / 2, () =>
		{
			// Move the button back up after the press
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

		// Call the callback if provided
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
