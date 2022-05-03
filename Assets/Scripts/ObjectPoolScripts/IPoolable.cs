using System;
using UnityEngine;

public interface IPoolable
{
    public Action<GameObject> ReleaseAction { set; get; }
    public PoolType PoolObjectType { set; get; }
}
