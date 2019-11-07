using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class WeaponCompData_authoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new WeaponComponentData
        {
            m_maxDistance = 50
        };
        var buildTime = new BuildingComponent
        {
            m_buildingTime = 1
        };
        dstManager.AddComponentData(entity,data);
        dstManager.AddComponentData(entity,buildTime);
        
    }
}

