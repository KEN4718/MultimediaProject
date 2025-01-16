using UnityEngine;

public class Waterwheel : MonoBehaviour
{
    public float rotationSpeed = 10f; // Rotation speed in degrees per second

    void Update()
    {
        // Rotate the waterwheel around its local Z-axis
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
