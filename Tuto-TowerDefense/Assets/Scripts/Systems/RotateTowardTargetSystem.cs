using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotateTowardTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        ComponentDataFromEntity<Translation> translations = GetComponentDataFromEntity<Translation>();
        
        Entities.ForEach((Entity _entity, ref RotationTowardTargetComponent _rotationTowardTarget, ref Translation
            _translation, ref TargetComponent _targetComponent, ref Rotation _rotation) =>
        {
            float3 targetPosition = float3.zero;

            if (EntityManager.Exists(_targetComponent.m_target))
            {
                targetPosition = translations[_targetComponent.m_target].Value;
            }

            float3 direction = targetPosition - _translation.Value;
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            _rotation.Value = Quaternion.Lerp(_rotation.Value, targetRotation, _rotationTowardTarget
            .m_rotationSpeed*Time.deltaTime);
        });
    }
}
