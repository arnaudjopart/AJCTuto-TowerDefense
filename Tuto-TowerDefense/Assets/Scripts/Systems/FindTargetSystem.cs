using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FindTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll(typeof(HasNoTarget)).WithNone<TurretInitializerComponent>().ForEach((Entity _entity, ref LocalToWorld _localToWorld, 
        ref Weapon 
        _weapon) =>
            {
                float3 positionOfTurret = _localToWorld.Position;
                float closestTargetDistance = _weapon.m_maxDetectionDistance;
                
                Entity closestTarget = Entity.Null;

                Entities.WithAll(typeof(Enemy)).ForEach(
                    (Entity _possibleTarget, ref Translation _possibleTargetTranslation) =>
                    {
                        if (math.distance(positionOfTurret, _possibleTargetTranslation.Value) < closestTargetDistance)
                        {
                            closestTargetDistance = math.distance(positionOfTurret, _possibleTargetTranslation.Value);
                            closestTarget = _possibleTarget;
                        }
                    });

                if (EntityManager.Exists(closestTarget))
                {
                    PostUpdateCommands.RemoveComponent(_entity,typeof(HasNoTarget));
                    PostUpdateCommands.SetComponent(_entity, new RotationTowardTargetComponent
                    {
                        m_rotationSpeed = 10,
                        m_target = closestTarget
                    });
                    _weapon.m_target = closestTarget;
                }
            });
        
    }
}
