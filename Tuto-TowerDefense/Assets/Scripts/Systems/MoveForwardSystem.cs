using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveForwardSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref MoveToPosition _moveToPosition, ref Translation _translation,
            ref Rotation _rotation) =>
        {
            float3 directionOfMove = math.mul(_rotation.Value, Vector3.forward);
            _translation.Value += directionOfMove * _moveToPosition.m_speed * Time.deltaTime;
        });
    }
}
