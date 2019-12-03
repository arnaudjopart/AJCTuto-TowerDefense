using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TurretInitializerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TurretInitializerComponent>().ForEach(_entity =>
        {
            PostUpdateCommands.RemoveComponent(_entity, typeof(TurretInitializerComponent));
        });
    }
}

public struct TurretInitializerComponent : IComponentData
{
    
}
