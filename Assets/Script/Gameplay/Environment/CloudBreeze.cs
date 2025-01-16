using UnityEngine;

public class CloudBreeze : MonoBehaviour
{
    [Header("Breeze Settings")]
    public Vector3 moveDirection = new Vector3(1f, 0f, 0f); // Direction of the breeze
    public float moveDistance = 2f;        // Maximum distance the cloud moves
    public float moveSpeed = 1f;           // Speed of the breeze movement

    private Vector3 startPosition;         // Initial position of the cloud

    void Start()
    {
        // Save the starting position
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the offset using a sine wave
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        // Apply the offset to the starting position along the move direction
        transform.position = startPosition + moveDirection.normalized * offset;
    }
}
