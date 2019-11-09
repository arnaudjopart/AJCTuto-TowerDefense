using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public SpawnerECS m_entitySpawner;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] spawnPositions = new Vector3[100];
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            float randomAngle = UnityEngine.Random.Range(0, Mathf.PI*2);
            float randomDistance = UnityEngine.Random.Range(50, 300);
            Vector3 spawPosition = new Vector3(randomDistance * math.sin(randomAngle), 0,
                randomDistance * math.cos(randomAngle));

            spawnPositions[i] = spawPosition;
        }

        m_entitySpawner.CreateTanksAt(spawnPositions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
