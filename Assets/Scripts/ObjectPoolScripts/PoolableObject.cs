using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour, IPoolable
{
    [SerializeField] private PoolType poolObjectType;

    /*
        Add more interaction when return to pool. HERE
    */
    public Action<GameObject> ReleaseAction { set; get; } //Invoke this when you need to return this.gameObject into the pool

    public PoolType PoolObjectType
    {
        get => poolObjectType;
        set => poolObjectType = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        ReleaseAction?.Invoke(this.gameObject);
    }
}
