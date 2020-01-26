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
                Debug.Log("ShootTarget");
                PostUpdateCommands.AddComponent(shootAction.m_shooter,new IsReloading
                {
                    m_reloadDuration = 2f
                });
                PostUpdateCommands.RemoveComponent(shootAction.m_shooter, typeof(IsShooting));
                PostUpdateCommands.DestroyEntity(shootAction.m_target);
            }
        }
    }
}