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

    public void ReturnToPoolAfter(float second)
    {
        StartCoroutine(ReturnToPoolAfterHelper(second));
    }

    private IEnumerator ReturnToPoolAfterHelper(float second)
    {
        yield return new WaitForSeconds(second);
        ReleaseAction?.Invoke(this.gameObject);
    }
}
