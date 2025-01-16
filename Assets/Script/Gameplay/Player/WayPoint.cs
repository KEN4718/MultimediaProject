using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPoint : MonoBehaviour
{
	public Transform father;
	public int type;                // The type of this WayPoint
	private int priority = 0;

	public bool isExplored;
	public WayPoint exploredFrom;

	public bool realPos = true;     // If true, use the actual position for pathfinding
	public float fakeX, fakeY;      // Fake coordinates for pathfinding if realPos is false

	public List<WayPoint> logicalNeighbors = new List<WayPoint>(); // Logical neighbors to bridge gaps

	private Material runtimeMaterial;   // Material instance used at runtime
	private Color originalColor;        // Original color of the material

	[Range(0f, 1f)] public float brightnessIncrease = 0.2f; // How much brighter to make the color on hover

	private void Awake()
	{
		father = transform.parent;

		// Cache the runtime material and store its original color
		Renderer renderer = GetComponent<Renderer>();
		if (renderer != null)
		{
			runtimeMaterial = renderer.material; // Creates an instance of the material at runtime
			originalColor = runtimeMaterial.color;
		}
	}

	public Vector2 GetPosition()    // Converts X, Z plane to X, Y coordinates
	{
		if (realPos)
		{
			return new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)); // Ensure coordinates are integers
		}
		else
		{
			return new Vector2(Mathf.RoundToInt(fakeX), Mathf.RoundToInt(fakeY));
		}
	}

	// Method to change the type with a priority check
	public void SetType(int newType, int newPriority)
	{
		if (newPriority >= priority)
		{
			type = newType;
			priority = newPriority; // Update the priority
		}
	}

	// Method to reset priority (optional, depending on your logic)
	public void ResetPriority()
	{
		priority = 0; // Reset to default priority
	}

	private void OnMouseEnter()
	{
		// When the mouse enters the cube, brighten the material's color
		if (runtimeMaterial != null)
		{
			Color brightenedColor = originalColor * (1f + brightnessIncrease);
			brightenedColor.a = originalColor.a; // Preserve alpha
			runtimeMaterial.color = brightenedColor;
		}
	}

	private void OnMouseExit()
	{
		// When the mouse leaves the cube, reset the material's color
		if (runtimeMaterial != null)
		{
			runtimeMaterial.color = originalColor;
		}
	}

	private void OnDrawGizmos()
	{
		// Draw the WayPoint label
		Handles.color = Color.black;
		Handles.Label(transform.position + Vector3.up * 0.5f, $"{type}");
	}
}
