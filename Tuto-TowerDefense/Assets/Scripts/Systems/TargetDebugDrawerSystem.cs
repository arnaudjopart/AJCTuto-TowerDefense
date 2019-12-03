using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TargetDebugDrawerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref Weapon _weapon, ref LocalToWorld _localToWorld) =>
        {
            if (EntityManager.Exists(_weapon.m_target))
            {
                if (EntityManager.HasComponent<Translation>(_weapon.m_target))
                {
                    Translation translationOfTarget = EntityManager.GetComponentData<Translation>(_weapon.m_target);
                    Debug.DrawLine(_localToWorld.Position, translationOfTarget.Value);
                }
            }
        });
    }
}
