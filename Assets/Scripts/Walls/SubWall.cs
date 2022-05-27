using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubWall : MonoBehaviour, IDamageable
{
    [HideInInspector] public int Row;
    [HideInInspector] public int Col;
    public string tankGrenadeLayerMask = "TankGrenade";

    private Wall wall;

    public float Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private void Awake()
    {
        wall = GetComponentInParent<Wall>();
    }

    private void Start()
    {
        registerToEvent(true);
    }

    private void OnDestroy()
    {
        registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            wall.OnSubWallDestroy += OnSubWallDestroy;
        }
        else
        {
            wall.OnSubWallDestroy -= OnSubWallDestroy;
        }
    }

    private void OnSubWallDestroy(int row, int col)
    {
        if (Row == row && Col == col)
        {
            Destroy(gameObject);
        }
    }

    public void SetRowAndCol(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public void TakeDamage(float damage, byte damageDealerID, Vector3? damageSourcePos = null)
    {
        wall.TakeDamage(damage, damageDealerID, damageSourcePos);
    }

    public void Die()
    {
        throw new NotImplementedException();
    }
}
