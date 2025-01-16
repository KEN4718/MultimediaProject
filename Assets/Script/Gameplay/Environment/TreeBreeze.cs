using UnityEngine;

public class TreeBreeze : MonoBehaviour
{
    [Header("Breeze Settings")]
    public float swaySpeed = 1.0f;    // Speed of the swaying motion
    public float swayAngle = 5.0f;    // Maximum sway angle (in degrees)
    public Vector3 swayAxis = Vector3.right; // Axis to sway around (e.g., Vector3.right or Vector3.forward)

    private Quaternion initialRotation;

    void Start()
    {
        // Save the initial rotation of the tree
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Calculate the swaying angle using a sine wave
        float angle = Mathf.Sin(Time.time * swaySpeed) * swayAngle;

        // Apply the rotation around the desired axis
        transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, swayAxis);
    }
}
