using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public SpawnerECS m_entitySpawner;

    public int m_nbOfInstanceToSpawn;

    
    public float m_minDistance;
    public float m_maxDistance;
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        Vector3[] spawnPositions = new Vector3[m_nbOfInstanceToSpawn];
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            float randomAngle = UnityEngine.Random.Range(0, Mathf.PI * 2);
            float randomDistance = UnityEngine.Random.Range(m_minDistance, m_maxDistance);
            Vector3 spawPosition = new Vector3(randomDistance * math.sin(randomAngle), 0,
                randomDistance * math.cos(randomAngle));

            spawnPositions[i] = spawPosition;
        }

        m_entitySpawner.CreateTanksAt(spawnPositions);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnEnemies();
        }
    }
}
