using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class SpawnerECS : MonoBehaviour
{
    public GameObject m_turretPrefab;

    public GameObject m_tankPrefab;
    
    // Start is called before the first frame update
    private void Awake()
    {
        m_entityManager = World.Active.EntityManager;
        TurretShootActionsNativeQueue = new NativeQueue<TurretShootTankAction>(Allocator.Persistent);
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
        m_entityManager.AddComponentData(instance, new AutoRotateAroundYAxis
        {
            m_speedOfRotation = math.radians(0)
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
                m_rotationSpeed = .5f
            });
            m_entityManager.AddComponentData(entity, new Enemy());
            m_entityManager.AddComponentData(entity, new MoveToPosition()
            {
                m_position = Vector3.zero,
                m_speed = 5f
            });
            m_entityManager.AddComponentData(entity, new Health()
            {
                Value = 50
            });
        }
    }


    public static NativeQueue<TurretShootTankAction> TurretShootActionsNativeQueue; 
    private EntityManager m_entityManager;
}

public struct TurretShootTankAction
{
    public Entity m_turret;
    public Entity m_target;
    public int m_damages;
}

public struct AutoRotateAroundYAxis : IComponentData
{
    public float m_speedOfRotation;
}

public struct RotateTowardTargetComponent: IComponentData
{
    public Entity m_target;
    public float m_rotationSpeed;
}

public struct MoveToPosition : IComponentData
{
    public float3 m_position;
    public float m_speed;
}

public class AutoRotateAroundY : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref AutoRotateAroundYAxis _rotateAroundY, ref Rotation _rotation) =>
        {
            _rotation.Value = math.mul(math.normalize(_rotation.Value), quaternion.AxisAngle(math.up(), _rotateAroundY
                                                                                                            .m_speedOfRotation *
                                                                                                        Time
                                                                                                            .deltaTime));
        });
    }
}

public class RotateTowardTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        ComponentDataFromEntity<LocalToWorld> ltws = GetComponentDataFromEntity<LocalToWorld>(true);
        ComponentDataFromEntity<Translation> translations = GetComponentDataFromEntity<Translation>(true);
        
        Entities.WithNone<Parent>().ForEach((Entity _entity, ref RotateTowardTargetComponent _rotateToward, ref Translation 
        _translation, 
        ref Rotation _rotation, ref LocalToWorld _localToWorld) =>
        {
            float3 targetPosition = float3.zero;
            
            if (EntityManager.Exists(_rotateToward.m_target))
            {
                targetPosition = translations[_rotateToward.m_target].Value;
            }
            float3 direction = targetPosition - _localToWorld.Position;
            
            Quaternion targetRotationInWorldSpace = Quaternion.LookRotation(direction);
            
            _rotation.Value = Quaternion.Lerp(_rotation.Value,targetRotationInWorldSpace,_rotateToward.m_rotationSpeed*Time.deltaTime);
            
        });
        
        Entities.ForEach((Entity _entity, ref RotateTowardTargetComponent _rotateToward, ref Translation 
                _translation, ref Rotation _rotation, ref LocalToWorld _localToWorld, ref Parent _parent) =>
        {
            if (EntityManager.Exists(_rotateToward.m_target)&&EntityManager.HasComponent<Translation>(_rotateToward.m_target))
            {
                //Translation targetTranslation = EntityManager.GetComponentData<Translation>(_rotateToward.m_target);
                float3 directionLTW = translations[_rotateToward.m_target].Value - _localToWorld.Position;
                //float3 directionLTW = targetTranslation.Value - _localToWorld.Position;
                
                Quaternion targetRotationLTW = Quaternion.LookRotation(directionLTW);

                //LocalToWorld parentLTW = EntityManager.GetComponentData<LocalToWorld>(_parent.Value);
                LocalToWorld parentLTW = ltws[_parent.Value];
                quaternion parentWorldRotation = Quaternion.LookRotation(parentLTW.Forward, parentLTW.Up);

                quaternion appliedRotation = math.mul(targetRotationLTW, math.inverse(parentWorldRotation));
                
                _rotation.Value = Quaternion.Lerp(_rotation.Value, appliedRotation,
                    _rotateToward.m_rotationSpeed * Time.deltaTime);

            }
            
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
                //Debug.Log("Looking for potential target");
                if (math.distance(position, _possibleTargetTranslation.Value) < closestTargetDistance)
                {
                    //Debug.Log("found new potential target");
                    closestTargetDistance = math.distance(position, _possibleTargetTranslation.Value);
                    closestTarget = _possibleTarget;
                }
                
            });

            if (EntityManager.Exists(closestTarget))
            {
                //Debug.Log("found new target");
                PostUpdateCommands.RemoveComponent(_entity, typeof(HasNoTarget));
                PostUpdateCommands.SetComponent(_entity, new RotateTowardTargetComponent
                {
                    m_target = closestTarget,
                    m_rotationSpeed = 10
                });
                _weaponComponent.m_target = closestTarget;

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
                    //Debug.Log("CheckIfNoTarget -> HasNoTarget");
                    PostUpdateCommands.AddComponent(_entity, new HasNoTarget());
                }
            });
    }
}
public struct WeaponComponentData : IComponentData
{
    public Entity m_target;
    public float m_maxDistance;
    public Vector3 m_nosePosition;
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
                if (EntityManager.HasComponent<Translation>(_weaponComponent.m_target))
                {
                    Translation targetTranslation = EntityManager.GetComponentData<Translation>(_weaponComponent.m_target);
                    //Debug.Log("Target Position: "+targetTranslation.Value);
                    Debug.DrawLine(_localToWorld.Position, targetTranslation.Value);
                }
                
            }
        });
    }
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class TurretBuildSystem : ComponentSystem
{
    
    protected override void OnUpdate()
    {
       Entities.ForEach((Entity _entity, ref BuildingComponent _buildingComponent, ref SetupBuild _setupBuild) =>
            {
                _buildingComponent.m_currentTime += Time.deltaTime;
                if (_buildingComponent.m_currentTime > _buildingComponent.m_buildingTime)
                {
                    //Debug.Log("Is ready");
                    
                    DynamicBuffer<Child> children = EntityManager.GetBuffer<Child>(_entity);
                    Debug.Log(children.Length);
                    foreach (var child in children)
                    {
                        //Debug.Log("Looking for nose");
                        if (EntityManager.HasComponent(child.Value, typeof(BarrelNose)))
                        {
                            //Debug.Log("Found Nose");
                            _setupBuild.m_nose = child.Value;
                        }
                    }
                    
                    PostUpdateCommands.RemoveComponent(_entity, typeof(BuildingComponent));
                }
            });
        
    }
}

public struct BarrelNose : IComponentData
{
}

public struct SetupBuild : IComponentData
{
    public Entity m_parent;
    public Entity m_nose;
}

public class TurretFireSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone(typeof(ReloadComponent)).ForEach((Entity _entity, ref WeaponComponentData _weaponComponent) =>
        {
            if (!EntityManager.Exists(_weaponComponent.m_target)) return;
            
            //Debug.Log("TurretFireSystem");
           
            SpawnerECS.TurretShootActionsNativeQueue.Enqueue(new TurretShootTankAction
            {
                m_turret = _entity,
                m_target = _weaponComponent.m_target,
                m_damages = 10
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
                //Debug.Log("ReloadSystem");
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


public class ApplyShootSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //ComponentDataFromEntity<Health> healths = GetComponentDataFromEntity<Health>(true);
        
        while (SpawnerECS.TurretShootActionsNativeQueue.TryDequeue(out var action))
        {
            
            if (EntityManager.Exists(action.m_target) && EntityManager.Exists(action.m_turret))
            {
                Debug.Log("ApplyShootSystem");
                if (EntityManager.HasComponent<Health>(action.m_target))
                {
                    Vector3 fxSpawnPosition = GetSpawnPosition(action.m_turret);
                    FXManager.SpawnFXAt(fxSpawnPosition);
                    Health enemyHealth = EntityManager.GetComponentData<Health>(action.m_target);
                    WeaponComponentData weaponComponentData = EntityManager.GetComponentData<WeaponComponentData>
                        (action.m_turret);
                    
                    enemyHealth.Value -= action.m_damages;

                    if (enemyHealth.Value <= 0)
                    {
                        EntityManager.DestroyEntity(action.m_target);
                    }
                    else
                    {
                        PostUpdateCommands.SetComponent(action.m_target, enemyHealth);
                    }
                }
                
                 
                PostUpdateCommands.AddComponent(action.m_turret, new ReloadComponent
            {
                m_reloadTime =.2f
            });
                
            }
        }
        
    }

    private Vector3 GetSpawnPosition(Entity _actionTurret)
    {
        Vector3 res = Vector3.zero;

        if (EntityManager.HasComponent<SetupBuild>(_actionTurret))
        {
            SetupBuild build = EntityManager.GetComponentData<SetupBuild>(_actionTurret);
            res = math.mul(EntityManager.GetComponentData<Rotation>(build.m_nose).Value, EntityManager
                .GetComponentData<LocalToWorld>(build
                    .m_nose).Position);
        }
        
        return res;
    }
}

public struct Health : IComponentData
{
    public int Value;
}



