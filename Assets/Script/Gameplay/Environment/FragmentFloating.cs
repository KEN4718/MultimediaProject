using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentFloating : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 0.1f; // The height of the floating motion
    public float floatSpeed = 1f; // Speed of the floating motion

    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Rotation speed on X, Y, Z axes

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position
        startPosition = transform.position;
    }

    void Update()
    {
        // Floating motion
        float newY = (startPosition.y + 0.5f) + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotating motion
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
