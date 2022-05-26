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


    public void Die()
    {
        OnDie?.Invoke();
    }

    public void TakeDamage(float damage, byte damageDealerID)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }
}
