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
    public Transform m_nose;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new WeaponComponentData
        {
            m_maxDetectionDistance = 50,
            //m_nosePosition = m_nose.position
        };
        var initComp = new InitComp
        {
            
        };
        
        // Partie #2
        var buildSetup = new PrefabStructure()
        {
            
        };
        // Partie #2
        var rotationToward =  new RotateTowardTargetComponent
        {
            m_rotationSpeed = 1
        };
        dstManager.AddComponentData(entity,data);
        dstManager.AddComponentData(entity,initComp);
        dstManager.AddComponentData(entity,rotationToward);
        dstManager.AddComponentData(entity,buildSetup);
        
    }
}

