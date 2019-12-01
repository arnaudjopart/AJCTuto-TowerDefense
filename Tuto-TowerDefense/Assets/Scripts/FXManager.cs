using Unity.Mathematics;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    private static FXManager Instance;
    public Pool m_shotFxPoolSystem;
    public Pool m_explosionsFxPoolSystem;

    private void Awake()
    {
        Instance = this;
    }

    public static void SpawnFXAt(Vector3 _position)
    {
        
        Instance.m_shotFxPoolSystem.ActivateFirstAvailableItemAt(_position);
        
    }

    public static void SpawnExplosionAt(float3 _value)
    {
        Instance.m_explosionsFxPoolSystem.ActivateFirstAvailableItemAt(_value);
    }

    
}