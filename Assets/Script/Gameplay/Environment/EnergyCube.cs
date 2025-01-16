using UnityEngine;

public class EnergyCube : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 10.0f; // Speed of rotation (degrees per second)

    [Header("Electric Spark Settings")]
    public GameObject electricSparkPrefab; // Prefab for the electric sparks
    public Transform[] sparkSpawnPoints;   // Points around the cube to spawn sparks
    public float sparkInterval = 1.0f;     // Time interval between sparks

    private float sparkTimer;

    void Update()
    {
        // Rotate the cube
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Handle electric sparks
        sparkTimer += Time.deltaTime;
        if (sparkTimer >= sparkInterval)
        {
            SpawnElectricSpark();
            sparkTimer = 0f; // Reset the timer
        }
    }

    private void SpawnElectricSpark()
    {
        if (electricSparkPrefab != null && sparkSpawnPoints.Length > 0)
        {
            // Choose a random spawn point
            Transform spawnPoint = sparkSpawnPoints[Random.Range(0, sparkSpawnPoints.Length)];

            // Instantiate the electric spark prefab
            GameObject spark = Instantiate(electricSparkPrefab, spawnPoint.position, Quaternion.identity);

            // Optionally, destroy the spark after a short duration
            Destroy(spark, 1.0f);
        }
    }
}
