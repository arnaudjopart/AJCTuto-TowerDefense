using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpawnerECS : MonoBehaviour
{
    public GameObject m_turretPrefab;

    public GameObject m_tankPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        m_entityManager = World.Active.EntityManager;
        CreateTurretAt(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Entity CreateEntityFromPrefab(GameObject _prefab, Vector3 _spawnPosition)
    {
        var ECSPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_prefab, World.Active);
        var instanceECS = m_entityManager.Instantiate(ECSPrefab);
        
        m_entityManager.SetComponentData(instanceECS, new Translation{Value = _spawnPosition});

        return instanceECS;
    }

    public void CreateTurretAt(Vector3 _spawnPosition)
    {
        Entity instance = CreateEntityFromPrefab(m_turretPrefab, _spawnPosition);
    }

    public void CreateTankAt(Vector3 _spawnPosition)
    {
        Entity instance = CreateEntityFromPrefab(m_tankPrefab, _spawnPosition);
    }

    private EntityManager m_entityManager;
}





