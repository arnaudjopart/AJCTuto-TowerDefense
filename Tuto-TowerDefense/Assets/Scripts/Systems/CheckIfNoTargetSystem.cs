using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CheckIfNoTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone(typeof(HasNoTarget)).ForEach((Entity _entity, ref Weapon _weapon) =>
        {
            if (EntityManager.Exists(_weapon.m_target) == false)
            {
                PostUpdateCommands.AddComponent(_entity,new HasNoTarget()); 
            }
        });
    }
}

public struct HasNoTarget : IComponentData
{
    
}
