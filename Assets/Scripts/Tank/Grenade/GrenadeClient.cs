using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeClient : MonoBehaviour
{
    [SerializeField] private GrenadeExplosionInformation grenadeExplosionInformation;

    public static GrenadeClient Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_GRENADE_EXPLOSION += OnClientReceivedGrenadeExplosionMessage;
        }
        else
        {
            NetUtility.C_GRENADE_EXPLOSION -= OnClientReceivedGrenadeExplosionMessage;
        }
    }

    private void OnClientReceivedGrenadeExplosionMessage(NetMessage message)
    {
        HandleGrenadeExplosionMessage(message as NetGrenadeExplosion);
    }

    private void HandleGrenadeExplosionMessage(NetGrenadeExplosion message)
    {
        Collider[] colliders = Physics.OverlapSphere(message.ExplosionPosition, grenadeExplosionInformation.ExplosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                float damage = grenadeExplosionInformation.Damage;
                if (collider.TryGetComponent<TankInformation>(out TankInformation tankInformation))
                {
                    damage = message.Team != tankInformation.Player.Team ? damage : grenadeExplosionInformation.SameTeamDamage;
                }

                damageable.TakeDamage(damage, message.ID, message.ExplosionPosition);
            }
        }
    }
}

