using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pool : MonoBehaviour
{
    private GameObject poolObjectPrefab;
    public ObjectPool<GameObject> ObjectPool;
    public PoolType PoolObjectType;

    public GameObject Get()
    {
        return ObjectPool.Get();
    }

    public void InitPool(GameObject objectPrefab)
    {
        this.poolObjectPrefab = objectPrefab;
        ObjectPool = new ObjectPool<GameObject>(CreateFunction, OnGetObject, OnReleaseObject, OnDestroyObject);
        PoolObjectType = poolObjectPrefab.GetComponent<PoolableObject>().PoolObjectType;
    }

    private void OnDestroyObject(GameObject destroyedObject)
    {
        Destroy(destroyedObject.gameObject);
    }

    private void OnReleaseObject(GameObject releasedObject)
    {
        if (releasedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
        }
        releasedObject.transform.position = this.transform.position;
        releasedObject.gameObject.SetActive(false);
    }

    private void OnGetObject(GameObject getObject)
    {
        getObject.gameObject.SetActive(true);
    }

    private GameObject CreateFunction()
    {
        GameObject createdObject = Instantiate(poolObjectPrefab, this.gameObject.transform);
        createdObject.GetComponent<PoolableObject>().ReleaseAction += OnObjectReleaseAction; // add OnObjectReleaseAction to ReleaseAction of the createdObject
        return createdObject;
    }

    private void OnObjectReleaseAction(GameObject obj)
    {
        ObjectPool.Release(obj);
    }
}