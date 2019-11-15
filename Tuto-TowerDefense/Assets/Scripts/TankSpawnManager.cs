using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawnManager : MonoBehaviour
{
    public EntitySpawner m_entitySpawner;
    public int m_nbOfEntitiesToSpawn;

    public float m_minDistanceOfSpawn;
    public float m_maxDistanceOfSpawn;
    // Start is called before the first frame update
    void Start()
    {
        Vector3[] spawnPositions = new Vector3[m_nbOfEntitiesToSpawn];

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            float randomAngle = UnityEngine.Random.Range(0, Mathf.PI * 2);
            float randomDistance = UnityEngine.Random.Range(m_minDistanceOfSpawn, m_maxDistanceOfSpawn);
            Vector3 spawnPosition = new Vector3(randomDistance*Mathf.Sin(randomAngle),0,randomDistance*Mathf.Cos
            (randomAngle));

            spawnPositions[i] = spawnPosition;
        }
        
        m_entitySpawner.CreateTanksAt(spawnPositions);
        
    }
    
}
