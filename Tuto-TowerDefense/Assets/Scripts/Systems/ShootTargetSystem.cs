using Unity.Entities;
using Unity.Rendering;

public class ShootTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<IsShootingTag>().ForEach((Entity _entity, ref Weapon _weapon) =>
        {
            RenderMesh mesh = EntityManager.GetSharedComponentData<RenderMesh>(_entity);
            mesh.material = EntitySpawner.GetShootMaterial();
            EntityManager.SetSharedComponentData(_entity,mesh);
            EntitySpawner.TurretShootActionsQueue.Enqueue(new TurretShootAction
            {
                m_shooter = _entity,
                m_target = _weapon.m_target
            });
            
        });
    }
}

public struct IsShootingTag : IComponentData
{
}