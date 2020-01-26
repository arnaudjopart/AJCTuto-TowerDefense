using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ActivateTurretFireSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<Weapon>().WithNone(typeof(IsShootingTag),typeof(IsReloadingTag)).ForEach((Entity _entity,
            ref RotationTowardTargetComponent _rotationToward, ref LocalToWorld _localToWorld) =>
        {
            if (EntityManager.Exists(_rotationToward.m_target))
            {
                var targetTranslation = EntityManager.GetComponentData<Translation>(_rotationToward.m_target);
                var targetDirectionInWorldSpace = targetTranslation.Value - _localToWorld.Position;
                if (!(Vector3.Angle(_localToWorld.Forward, targetDirectionInWorldSpace) < 10)) return;
                PostUpdateCommands.AddComponent(_entity,new IsShootingTag());
            }
           
        });
    }
}

