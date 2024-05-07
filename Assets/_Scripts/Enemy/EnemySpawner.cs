using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform target; // Add this line
    public float spawnInterval = 2f;
    public float spawnRadius = 5f;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", spawnInterval, spawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = 0f;
        Instantiate(enemyPrefab, target.position + spawnPosition, Quaternion.identity); // Modify this line
    }
}
