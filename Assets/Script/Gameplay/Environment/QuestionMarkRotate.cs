using UnityEngine;

public class RotatingScript : MonoBehaviour
{
    // Speed of rotation (can be adjusted in the Inspector)
    public float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
