using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    { 
        public GameObject prefab; 
        public int size;          
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab.name, objectPool);
        }
    }

    public GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(name))
        {
            Debug.LogWarning($"Pool name {name} dont exit");
            return null;
        }

        GameObject objectToSpawn;

        if (poolDictionary[name].Count > 0)
        {
            objectToSpawn = poolDictionary[name].Dequeue();
        }
        else
        {
            Pool poolConfig = pools.Find(p => p.prefab.name == name);
            objectToSpawn = Instantiate(poolConfig.prefab, transform);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);

        return objectToSpawn;
    }

    public void ReturnToPool(string name, GameObject obj)
    {
        obj.SetActive(false);
        poolDictionary[name].Enqueue(obj);
    }
}