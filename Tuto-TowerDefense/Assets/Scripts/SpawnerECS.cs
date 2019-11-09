using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnerECS : MonoBehaviour
{
    public GameObject m_turretPrefab;

    public GameObject m_tankPrefab;
    
    // Start is called before the first frame update
    private void Awake()
    {
        m_entityManager = World.Active.EntityManager;
    }
    
    private Entity CreateEntityFromPrefab(GameObject _prefab, Vector3 _spawnPosition)
    {
        var ECSPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_prefab, World.Active);
        var instanceECS = m_entityManager.Instantiate(ECSPrefab);
        
        m_entityManager.SetComponentData(instanceECS, new Translation{Value = _spawnPosition});

        return instanceECS;
    }
    
    private NativeArray<Entity> CreateEntitiesFromPrefab(GameObject _prefab, Vector3[] _spawnPositions)
    {
        NativeArray<Entity> nativeArray = new NativeArray<Entity>(_spawnPositions.Length,Allocator.Temp);
        
        var ECSPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_prefab, World.Active);
        
        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            var instanceECS = m_entityManager.Instantiate(ECSPrefab);
            m_entityManager.SetComponentData(instanceECS, new Translation{Value = _spawnPositions[i]});
            m_entityManager.SetComponentData(instanceECS, new Rotation{Value = Quaternion.identity});
            nativeArray[i] = instanceECS;
        }

        return nativeArray;
    }

    public void CreateTurretAt(Vector3 _spawnPosition)
    {
        Entity instance = CreateEntityFromPrefab(m_turretPrefab, _spawnPosition);
        m_entityManager.SetComponentData(instance, new Rotation
        {
            Value = Quaternion.Euler(0,0,0)
        });
        
    }

    public Entity CreateTankAt(Vector3 _spawnPosition)
    {
        Entity instance = CreateEntityFromPrefab(m_tankPrefab, _spawnPosition);
        return instance;
    }

    public void CreateTanksAt(Vector3[] _positions)
    {
        var tanks = CreateEntitiesFromPrefab(m_tankPrefab, _positions);

        foreach (var entity in tanks)
        {
            m_entityManager.AddComponentData(entity, new RotateTowardTargetComponent{
                m_targetPosition = Vector3.zero,
                m_rotationSpeed = .5f
            });
            m_entityManager.AddComponentData(entity, new Enemy());
            m_entityManager.AddComponentData(entity, new MoveToPosition()
            {
                m_position = Vector3.zero,
                m_speed = 5f
            });
        }
    }

    

    private EntityManager m_entityManager;
}

public struct RotateTowardTargetComponent: IComponentData
{
    public float3 m_targetPosition;
    public Entity m_target;
    public float m_rotationSpeed;
}

public struct MoveToPosition : IComponentData
{
    public float3 m_position;
    public float m_speed;
}


public class RotateTowardTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref RotateTowardTargetComponent _rotateToward, ref Translation _translation, 
        ref Rotation _rotation, ref LocalToWorld _localToWorld) =>
        {
            if (EntityManager.Exists(_rotateToward.m_target))
            {
                Translation targetTranslation = EntityManager.GetComponentData<Translation>(_rotateToward.m_target);
                _rotateToward.m_targetPosition = targetTranslation.Value;
            }
            
            float3 direction = _rotateToward.m_targetPosition - _localToWorld.Position;
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            //Quaternion localRotation = math.mul(_rotation.Value,
            _rotation.Value = Quaternion.Lerp(_rotation.Value,targetRotation,_rotateToward.m_rotationSpeed*Time.deltaTime);
        });
    }
}

/*public class MoveToPositionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref MoveToPosition _moveToPosition, ref Translation _translation, 
            ref Rotation _rotation) =>
        {
            float3 direction = math.normalize(_moveToPosition.m_position - _translation.Value);
            _translation.Value += direction * _moveToPosition.m_speed * Time.deltaTime;
        });
    }
}*/

public class MoveForwardSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref MoveToPosition _moveToPosition, ref Translation _translation, 
            ref Rotation _rotation) =>
        {
            float3 direction = math.mul(_rotation.Value,Vector3.forward);
            _translation.Value += direction * _moveToPosition.m_speed * Time.deltaTime;
        });
    }
}

public class FindTarget : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll(typeof(HasNoTarget)).WithNone(typeof(BuildingComponent)).ForEach((Entity _entity, ref LocalToWorld
         _localToWorld, ref WeaponComponentData _weaponComponent) =>
        {
            float3 position = _localToWorld.Position;
            float closestTargetDistance =_weaponComponent.m_maxDistance ;
            
            Entity closestTarget = Entity.Null;
            
            Entities.WithAll(typeof(Enemy)).ForEach((Entity _possibleTarget, ref Translation _possibleTargetTranslation) =>
            {
                if (math.distance(position, _possibleTargetTranslation.Value) < closestTargetDistance)
                {
                    Debug.Log("found new target");
                    closestTargetDistance = math.distance(position, _possibleTargetTranslation.Value);
                    closestTarget = _possibleTarget;
                }
                
            });

            if (EntityManager.Exists(closestTarget))
            {
                
                PostUpdateCommands.RemoveComponent(_entity, typeof(HasNoTarget));
                _weaponComponent.m_target = closestTarget;
                PostUpdateCommands.SetComponent(_entity, new RotateTowardTargetComponent
                {
                    m_target = closestTarget,
                    m_rotationSpeed = 10
                });
                
            }
        });
    }
}

public struct BuildingComponent : IComponentData
{
    public float m_buildingTime;
    public float m_currentTime;
}

public class CheckIfNoTarget : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone(typeof(HasNoTarget)).ForEach((Entity _entity, ref WeaponComponentData _weaponComponent) =>
            {
                if (EntityManager.Exists(_weaponComponent.m_target) == false)
                {
                    Debug.Log("CheckIfNoTarget -> HasNoTarget");
                    PostUpdateCommands.AddComponent(_entity, new HasNoTarget());
                }
            });
    }
}
public struct WeaponComponentData : IComponentData
{
    public Entity m_target;
    public float m_maxDistance;
}

public struct Enemy : IComponentData
{
}

public struct HasNoTarget : IComponentData
{
}

public class TargetDebugSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref WeaponComponentData _weaponComponent, 
        ref LocalToWorld _localToWorld) =>
        {
            if (EntityManager.Exists(_weaponComponent.m_target))
            {
                //Debug.Log("TargetDebugSystem -> Debug.DrawLine");
                //Debug.Log("Turret Position: "+_localToWorld.Position);
                
                Translation targetTranslation = EntityManager.GetComponentData<Translation>(_weaponComponent.m_target);
                //Debug.Log("Target Position: "+targetTranslation.Value);
                Debug.DrawLine(_localToWorld.Position, targetTranslation.Value);
            }
        });
    }
}

public class TurretBuildSystem : ComponentSystem
{
    
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref BuildingComponent _buildingComponent) =>
            {
                _buildingComponent.m_currentTime += Time.deltaTime;
                if (_buildingComponent.m_currentTime > _buildingComponent.m_buildingTime)
                {
                    Debug.Log("Is ready");
                    PostUpdateCommands.RemoveComponent(_entity, typeof(BuildingComponent));
                }
            });
    }
}

public class TurretFireSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone(typeof(ReloadComponent)).ForEach((Entity _entity, ref WeaponComponentData _weaponComponent) =>
        {
            if (!EntityManager.Exists(_weaponComponent.m_target)) return;
            
            PostUpdateCommands.DestroyEntity(_weaponComponent.m_target);
            PostUpdateCommands.AddComponent(_entity, new ReloadComponent
            {
                m_reloadTime =5
            });
        });
    }
}

public class ReloadSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref ReloadComponent _component) =>
        {
            if (_component.m_currentTimeSpend < _component.m_reloadTime)
            {
                _component.m_currentTimeSpend += Time.deltaTime;
            }
            else
            {
                PostUpdateCommands.RemoveComponent(_entity,typeof(ReloadComponent));
            }
        });
    }
}

public struct ReloadComponent : IComponentData
{
    public float m_reloadTime;
    public float m_currentTimeSpend;
}



