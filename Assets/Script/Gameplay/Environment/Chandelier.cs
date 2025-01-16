using UnityEngine;

public class ChandelierSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingSpeed = 1.0f;   // Speed of the swing
    public float swingAngle = 15.0f; // Maximum swing angle in degrees

    [Header("Axis of Swing")]
    public Vector3 swingAxis = Vector3.forward; // Axis of rotation (e.g., forward, right, or custom)

    private Quaternion initialRotation;

    void Start()
    {
        // Save the initial rotation of the chandelier
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Calculate the oscillating angle using a sine wave
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;

        // Apply the rotation around the desired axis
        transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, swingAxis);
    }
}
