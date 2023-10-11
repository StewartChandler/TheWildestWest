using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSpawner : MonoBehaviour
{
    public GameObject[] objectPrefabs; // Array to hold the 5 different object prefabs
    private float spawnInterval = 1.0f;
    private float spawnRadius = 14.0f;
    private float despawnTime = 20.0f;

    private float nextSpawnTime;

    private float randomSpawnVariance = 0.5f;

    private void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnRandomObject();
            nextSpawnTime = Time.time + spawnInterval + randomSpawnVariance;
        }
    }

    private void SpawnRandomObject()
    {
        // Calculate a random X position within the spawn radius
        float randomX = Random.Range(-spawnRadius, spawnRadius);
        float randomZ = Random.Range(-spawnRadius / 4, spawnRadius / 4);

        // Calculate a random rotation for the object
        Quaternion randomRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        // Calculate the spawn position
        Vector3 spawnPosition = transform.position + new Vector3(randomX, 0, randomZ);

        // Choose a random prefab from the objectPrefabs array
        GameObject selectedPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];

        // Instantiate the selected prefab at the calculated position
        GameObject spawnedObject = Instantiate(selectedPrefab, spawnPosition, randomRotation);


        // Change the random spawn variance
        randomSpawnVariance = Random.Range(-randomSpawnVariance, randomSpawnVariance);

        // Destroy the object after the specified despawnTime
        Destroy(spawnedObject, despawnTime);
    }
}
