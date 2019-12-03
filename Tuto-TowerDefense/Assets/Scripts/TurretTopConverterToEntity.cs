using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class TurretTopConverterToEntity : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var weapon = new Weapon
        {
            m_maxDetectionDistance = 50
        };
        var rotationToward = new RotationTowardTargetComponent
        {
            m_rotationSpeed = 1
        };
        
        var initializer = new TurretInitializerComponent();
        
        dstManager.AddComponentData(entity, weapon);
        dstManager.AddComponentData(entity, rotationToward);
        dstManager.AddComponentData(entity, initializer);
        
    }
}

public struct RotationTowardTargetComponent : IComponentData
{
    public float m_rotationSpeed;
    public Entity m_target;
}

public struct Weapon : IComponentData
{
    public float m_maxDetectionDistance;
    public Entity m_target;
}


