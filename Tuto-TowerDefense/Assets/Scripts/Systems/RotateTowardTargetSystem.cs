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
        ComponentDataFromEntity<LocalToWorld> ltws = GetComponentDataFromEntity<LocalToWorld>();
        
        //Case when rotating entity is not a child
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
        
        //Case when rotating entity is child of another entity
        Entities.ForEach((Entity _entity, ref RotationTowardTargetComponent _rotationTowardTarget, ref Translation
            _translation, ref Rotation _rotation, ref LocalToWorld _localToWorld, ref Parent _parent) =>
        {
            if (EntityManager.Exists(_rotationTowardTarget.m_target) &&
                EntityManager.HasComponent<Translation>(_rotationTowardTarget.m_target))
            {
                float3 targetDirectionInWorldSpace = translations[_rotationTowardTarget.m_target].Value -
                                                     _localToWorld.Position;
                Quaternion targetRotationInWorldSpace = Quaternion.LookRotation(targetDirectionInWorldSpace);

                LocalToWorld parentLTW = ltws[_parent.Value];
                quaternion parentWorldRotation = Quaternion.LookRotation(parentLTW.Forward, parentLTW.Up);

                quaternion relativeRotationOfTurret =
                    math.mul(targetRotationInWorldSpace, math.inverse(parentWorldRotation));
                
                _rotation.Value = Quaternion.Lerp(_rotation.Value, relativeRotationOfTurret, _rotationTowardTarget
                .m_rotationSpeed*Time.deltaTime);
            }
        });
        
    }
}











