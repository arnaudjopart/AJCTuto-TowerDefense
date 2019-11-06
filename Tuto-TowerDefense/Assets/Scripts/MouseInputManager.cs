using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : MonoBehaviour
{
    public Camera m_camera;

    public float m_maxRayCastDistance = 50f;

    public LayerMask m_layerMask;

    public SpawnerECS m_spawnerEcs;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = m_camera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, m_maxRayCastDistance, m_layerMask))
            {
                Vector3 spawnPosition = hit.point;
                m_spawnerEcs.CreateTurretAt(spawnPosition);
            }
        }
    }
}
