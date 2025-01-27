using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public PoolData poolData; 

    private Dictionary<ObjectType, Queue<GameObject>> poolDictionary; 

    void Awake()
    {
        poolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();

        foreach (PoolItem item in poolData.items)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < item.amount; i++)
            {
                GameObject obj = Instantiate(item.prefab, transform, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(item.type, objectPool);
        }
    }

    public T GetFromPool<T>(ObjectType type, Vector3 position, Quaternion rotation) where T : Component
    {
        if (!poolDictionary.ContainsKey(type))
        {
            return null;
        }

        if (poolDictionary[type].Count == 0)
        {
            PoolItem item = System.Array.Find(poolData.items, x => x.type == type);
            GameObject obj = Instantiate(item.prefab);
            obj.SetActive(false);
            poolDictionary[type].Enqueue(obj);
        }

        GameObject pooledObject = poolDictionary[type].Dequeue();
        pooledObject.SetActive(true);
        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;

        T component = pooledObject.GetComponent<T>();
        if (component == null)
        {
            ReturnToPool(type, pooledObject); // Hatalı nesneyi geri koy
            return null;
        }

        return component;
    }


    // Pool'a nesne geri koy
    public void ReturnToPool(ObjectType type, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            return;
        }

        obj.SetActive(false);
        poolDictionary[type].Enqueue(obj);
    }
}
