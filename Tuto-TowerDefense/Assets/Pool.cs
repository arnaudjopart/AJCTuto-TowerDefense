using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public PoolItem m_item;

    public int m_startItemNumber = 50;

    private List<PoolItem> m_items;

    // Start is called before the first frame update
    void Start()
    {
        m_items = new List<PoolItem>();
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < m_startItemNumber; i++)
        {
            m_items.Add(CreateItem());
        }
    }


    private PoolItem CreateItem()
    {
        PoolItem instance = Instantiate(m_item, transform) as PoolItem;

        instance.Init(this);

        return instance;

    }

    public PoolItem GetFirstAvailableItem()
    {
        foreach (var item in m_items)
        {
            if (item.gameObject.activeSelf == false)
            {
                return item;
            }
        }

        PoolItem newInstance = CreateItem();
        m_items.Add(newInstance);
        return newInstance;
    }

    public void ActivateFirstAvailableItemAt(Vector3 _spawnPosition)
    {
        PoolItem item = GetFirstAvailableItem();
        if (item != null)
        {
            item.ActivateAt(_spawnPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
