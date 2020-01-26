using Unity.Entities;
using UnityEngine;

public class ShootActionManagerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        while (EntitySpawner.TurretShootActionsQueue.TryDequeue(out var shootAction))
        {
            
            if (EntityManager.Exists(shootAction.m_shooter) && EntityManager.Exists(shootAction.m_target))
            {
                PostUpdateCommands.AddComponent(shootAction.m_shooter,new IsReloadingTag
                {
                    m_reloadDuration = 2f
                });
                PostUpdateCommands.RemoveComponent(shootAction.m_shooter, typeof(IsShootingTag));
                PostUpdateCommands.DestroyEntity(shootAction.m_target);
            }
        }
    }
}