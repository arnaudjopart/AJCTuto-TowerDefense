using UnityEngine;

public class FXManager : MonoBehaviour
{
    private static FXManager Instance;

    public GameObject m_fxShoot;
    private void Awake()
    {
        Instance = this;
    }

    public static void SpawnFXAt(Vector3 _position)
    {
        //print(_position);
        GameObject instance = Instantiate(Instance.m_fxShoot, _position,Quaternion.identity) as GameObject;
        
    }
}