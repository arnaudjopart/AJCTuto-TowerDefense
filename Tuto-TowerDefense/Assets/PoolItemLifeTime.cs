using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PoolItemLifeTime : MonoBehaviour
{
    public float m_lifeTime;

    private float m_currentLifeSpent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_currentLifeSpent += Time.deltaTime;
        if (m_currentLifeSpent > m_lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        m_currentLifeSpent = 0;
        
    }

    private void OnDisable()
    {
        m_currentLifeSpent = 0;
    }
}
