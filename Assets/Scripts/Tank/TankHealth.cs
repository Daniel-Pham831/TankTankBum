using UnityEngine;
using System;

public class TankHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private TankInformation localTankInfo;

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
        RegisterToEvent(true);
    }

    private void RegisterToEvent(bool confirm)
    {
        if (confirm)
        {
        }
        else
        {
        }
    }

    public void Die()
    {
        OnDie?.Invoke();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }
}