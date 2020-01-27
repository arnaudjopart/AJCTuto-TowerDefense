using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ReloadSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity _entity, ref IsReloading _isReloading) =>
        {
            RenderMesh mesh = EntityManager.GetSharedComponentData<RenderMesh>(_entity);
            mesh.material = EntitySpawner.GetDefaultTurretMaterial();
            EntityManager.SetSharedComponentData(_entity,mesh);
            _isReloading.m_currentReloadTime += Time.deltaTime;
            if (_isReloading.m_currentReloadTime > _isReloading.m_reloadDuration)
            {
                EntityManager.RemoveComponent(_entity,typeof(IsReloading));
            }

        });
            
    }
}

public struct IsReloading: IComponentData
{
    public float m_reloadDuration;
    public float m_currentReloadTime;
}