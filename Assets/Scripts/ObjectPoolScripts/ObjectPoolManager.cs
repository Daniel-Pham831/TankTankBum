using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    TGrenade,
    GrenadeExplosion,
    TankExplosion,
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Singleton { get; private set; }
    [SerializeField] private GameObject[] ObjectNeedToPool;
    private List<Pool> pools;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        pools = new List<Pool>();
        DontDestroyOnLoad(gameObject);
        CreatePools();
    }

    private void CreatePools()
    {
        foreach (GameObject go in ObjectNeedToPool)
        {
            CreateAPool(go);
        }
    }

    private void CreateAPool(GameObject go)
    {
        GameObject emptyObject = new GameObject($"{go.name}Pool");

        Pool pool = emptyObject.AddComponent<Pool>();
        pool.InitPool(go);
        pools.Add(pool);
    }

    public Pool GetPool(PoolType type)
    {
        foreach (Pool pool in pools)
        {
            if (pool.PoolObjectType == type)
                return pool;
        }

        return null;
    }
}
