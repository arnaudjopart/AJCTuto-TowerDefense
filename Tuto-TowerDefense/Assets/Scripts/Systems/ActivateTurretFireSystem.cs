using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ActivateTurretFireSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<Weapon>().WithNone(typeof(IsShootingTag),typeof(IsReloading)).ForEach((Entity _entity, ref
            RotationTowardTargetComponent _rotationToward, ref LocalToWorld _localToWorld) =>
        {
            if (EntityManager.Exists(_rotationToward.m_target))
            {
                var targetTranslation = EntityManager.GetComponentData<Translation>(_rotationToward.m_target);
                var targetDirectionInWorldSpace = targetTranslation.Value - _localToWorld.Position;

                if (!(Vector3.Angle(_localToWorld.Forward, targetDirectionInWorldSpace) < 10)) return;
                
                PostUpdateCommands.AddComponent(_entity, new IsShootingTag());
            }
        });
    }
}


public struct IsShootingTag : IComponentData
{
    
}

public struct IsReloading : IComponentData
{
    public float m_reloadDuration;
    public float m_currentReloadTime;
}
