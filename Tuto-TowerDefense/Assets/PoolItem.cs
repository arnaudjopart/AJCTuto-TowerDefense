using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Pool _pool)
    {
        gameObject.SetActive(false);
    }

    public void ActivateAt(Vector3 _spawnPosition)
    {
        transform.position = _spawnPosition;
        gameObject.SetActive(true);
    }
}
