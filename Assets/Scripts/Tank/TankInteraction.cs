using UnityEngine;
using System;

public class TankInteraction : MonoBehaviour, IDamageable
{
    [SerializeField] private TankInformation localTankInfo;
    [SerializeField] private GameObject tankGrenadePrefab;

    private float health;
    public float Health
    {
        get => health;
        set => health = value;
    }

    public Action OnTakeDamage;
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
        OnTakeDamage?.Invoke();
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }
}
