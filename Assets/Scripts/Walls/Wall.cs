using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IDamageable
{
    private float health;
    public float Health
    {
        get => health;
        set
        {
            health = value;
            OnCurrentHealthChanged?.Invoke(health);
        }
    }

    public Action<float> OnCurrentHealthChanged;
    public Action OnDie;

    private void Start()
    {
        Health = WallsData.Singleton.defaultWallHealth;
    }


    public void Die()
    {
        OnDie?.Invoke();

        Destroy(this.gameObject);
    }

    public void TakeDamage(float damage, byte damageDealerID, Vector3? damageSourcePos = null)
    {
        Health -= damage;
        Debug.Log(damageSourcePos);
        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }
}
