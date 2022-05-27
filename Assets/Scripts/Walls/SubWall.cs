using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubWall : MonoBehaviour, IDamageable
{
    public float Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void TakeDamage(float damage, byte damageDealerID, Vector3? damageSourcePos = null)
    {
        if (Vector3.Distance(transform.position, damageSourcePos.Value) <= WallsData.Singleton.EffectRadius)
        {
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        throw new NotImplementedException();
    }
}
