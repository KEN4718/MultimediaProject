using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatFloat : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatHeight = 0.5f;       // How much the boat floats up and down
    public float floatSpeed = 1.0f;        // Speed of the vertical bobbing motion
    public float tiltAngle = 5.0f;         // Maximum tilt angle for instability
    public float tiltSpeed = 1.5f;         // Speed of the tilting motion

    private Vector3 initialPosition;       // Initial position of the boat
    private Quaternion initialRotation;    // Initial rotation of the boat

    void Start()
    {
        // Store the initial position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Calculate the vertical bobbing motion
        float verticalOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Calculate the tilt angles for instability
        float tiltX = Mathf.Sin(Time.time * tiltSpeed) * tiltAngle;
        float tiltZ = Mathf.Cos(Time.time * tiltSpeed) * tiltAngle;

        // Apply the bobbing and tilting motions
        transform.position = initialPosition + new Vector3(0, verticalOffset, 0);
        transform.rotation = initialRotation * Quaternion.Euler(tiltX, 0, tiltZ);
    }
}
