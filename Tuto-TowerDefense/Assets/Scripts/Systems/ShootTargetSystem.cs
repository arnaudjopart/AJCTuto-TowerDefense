using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ShootTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<IsShootingTag>().ForEach((Entity _entity, ref Weapon _weapon) =>
        {
            RenderMesh mesh = EntityManager.GetSharedComponentData<RenderMesh>(_entity);
            mesh.material = EntitySpawner.GetShootingMaterial();
            EntityManager.SetSharedComponentData(_entity,mesh);
            
            EntitySpawner.TurretShootActionNativeQueue.Enqueue(new TurretShootAction
            {
                m_shooter = _entity,
                m_target = _weapon.m_target
            });
        });
    }
}
