using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_turretPrefab;
    [SerializeField] private GameObject m_tankPrefab;

    [SerializeField] private Material m_shootMaterial;
    [SerializeField] private Material m_defaultMaterial;

    private static EntitySpawner Instance; 
    private void Awake()
    {
        m_entityManager = World.Active.EntityManager;

        Instance = this;
        
        TurretShootActionNativeQueue = new NativeQueue<TurretShootAction>(Allocator.Persistent);
    }

    public static Material GetShootingMaterial()
    {
        return Instance.m_shootMaterial;
    }
    
    public static Material GetDefaultMaterial()
    {
        return Instance.m_defaultMaterial;
    }
    
    

    private Entity CreateEntityFromPrefab(GameObject _prefab, Vector3 _spawnPosition)
    {
        var entityFromPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_prefab, World.Active);
        var instance = m_entityManager.Instantiate(entityFromPrefab);
        
        m_entityManager.SetComponentData(instance, new Translation{Value = _spawnPosition});

        return instance;
    }

    private NativeArray<Entity> CreateEntitiesFromPrefab(GameObject _prefab, Vector3[] _spawnPositions)
    {
        NativeArray<Entity> nativeArray = new NativeArray<Entity>(_spawnPositions.Length, Allocator.Temp);
        var entityFromPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_prefab, World.Active);
        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            var instance = m_entityManager.Instantiate(entityFromPrefab);
            m_entityManager.SetComponentData(instance, new Translation{Value = _spawnPositions[i]});
            m_entityManager.SetComponentData(instance, new Rotation{Value = Quaternion.identity});
            nativeArray[i] = instance;
        }

        return nativeArray;

    }

    public Entity CreateTurretAt(Vector3 _spawnPosition)
    {
        Entity instance = CreateEntityFromPrefab(m_turretPrefab, _spawnPosition);
        return instance;
    }

    public Entity CreateTankAt(Vector3 _spawnPosition)
    {
        Entity instance = CreateEntityFromPrefab(m_tankPrefab, _spawnPosition);
        return instance;
    }

    public void CreateTanksAt(Vector3[] _spawnPositions)
    {
        var tanks = CreateEntitiesFromPrefab(m_tankPrefab, _spawnPositions);

        foreach (var tank in tanks)
        {
            //Add here ECS components
            
            m_entityManager.AddComponentData(tank,new RotationTowardTargetComponent
            {
                m_rotationSpeed = .5f
            });
            
            m_entityManager.AddComponentData(tank,new Enemy
            {
                
            });
            
            m_entityManager.AddComponentData(tank,new MoveToPosition
            {
                m_speed = 5f
            });
            
            m_entityManager.AddComponentData(tank,new TargetComponent()
            {
                
            });
            
        }
    }

    private EntityManager m_entityManager;

    public static NativeQueue<TurretShootAction> TurretShootActionNativeQueue;
}

public struct Enemy : IComponentData
{
    
}

public struct MoveToPosition : IComponentData
{
    public float m_speed;
    public Vector3 m_destination;
}

public struct TargetComponent : IComponentData
{
    public Entity m_target;
}

public struct TurretShootAction
{
    public Entity m_shooter;
    public Entity m_target;
}

















