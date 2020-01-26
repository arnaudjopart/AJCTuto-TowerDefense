using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ActivateTurretFireSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<Weapon>().WithNone(typeof(IsShooting)).ForEach((Entity _entity, ref Translation _translation,
            ref RotationTowardTargetComponent _rotationToward, ref LocalToWorld _localToWorld) =>
        {
            if (EntityManager.Exists(_rotationToward.m_target))
            {
                Translation targetTranslation = EntityManager.GetComponentData<Translation>(_rotationToward.m_target);
                float3 targetDirectionInWorldSpace = targetTranslation.Value - _localToWorld.Position;
                if (!(Vector3.Angle(_localToWorld.Forward, targetDirectionInWorldSpace) < 10)) return;
                Debug.Log("ReadyToShoot");
                PostUpdateCommands.AddComponent(_entity,new IsShooting());
            }
           
        });
    }
}