using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ObjectPool
{
    private PoolableObject prefab;
    private List<PoolableObject> avaiableObjectList;
    private ObjectPool(PoolableObject prefab, int size)
    {
        this.prefab = prefab;
        avaiableObjectList = new List<PoolableObject>(size);
    }
    public static ObjectPool CreateInstance(PoolableObject prefab, int size)
    {
        ObjectPool pool = new ObjectPool(prefab, size);

        GameObject poolObject = new GameObject(prefab.name + " Pool");
        pool.CreateObject(poolObject.transform, size);
        return pool;
    }
    private void CreateObject(Transform parent, int size)
    {
        for (int i = 0; i < size; i++)
        {
            PoolableObject poolableObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
            poolableObject.Parent = this;
            poolableObject.gameObject.SetActive(false);
        }
    }
    public void ReturnObjectToPool(PoolableObject poolableObject)
    {
        avaiableObjectList.Add(poolableObject);
        //Debug.Log($"Available object remain in pool: {avaiableObjectList.Count}");
    }
    public PoolableObject GetObject()
    {
        PoolableObject instance = avaiableObjectList[0];

        avaiableObjectList.RemoveAt(0);

        instance.gameObject.SetActive(true);

        return instance;
    }
    public bool CheckAvaiableObjectList()
    {
        return avaiableObjectList.Any();
    }
    public List<PoolableObject> GetAvailableObjectList()
    {
        return avaiableObjectList;
    }
}
