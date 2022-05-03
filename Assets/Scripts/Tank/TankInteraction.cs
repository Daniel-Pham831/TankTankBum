using UnityEngine;
using System;

public class TankInteraction : MonoBehaviour, IDamageable, IFireable
{
    [SerializeField] private float health;
    [SerializeField] private float rateOfFire;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private TankInformation localTankInfo;
    [SerializeField] private GameObject tankGrenadePrefab;

    public float Health
    {
        get => health;
        set => health = value;
    }

    public float RateOfFire
    {
        get => rateOfFire;
        set => rateOfFire = value;
    }
    public float ProjectileSpeed
    {
        get => projectileSpeed;
        set => projectileSpeed = value;
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
            NetUtility.C_T_FIRE_INPUT += OnClientReceivedTFireInputMessage;
        }
        else
        {
            NetUtility.C_T_FIRE_INPUT -= OnClientReceivedTFireInputMessage;
        }
    }

    private void OnClientReceivedTFireInputMessage(NetMessage message)
    {
        
    }

    public void Die()
    {
        OnDie?.Invoke();
    }

    public void Fire()
    {
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
