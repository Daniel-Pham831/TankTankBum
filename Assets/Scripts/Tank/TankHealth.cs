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

    public void Die() { }
    private void Die(byte damageDealerID)
    {
        // Get explosion FX from pool
        Pool tankExplosionPool = ObjectPoolManager.Singleton.GetPool(PoolType.TankExplosion);
        GameObject tankExplosionFX = tankExplosionPool.Get();

        // Move to explosion position and play
        tankExplosionFX.transform.position = this.transform.position;
        tankExplosionFX.GetComponent<ParticleSystem>().Play();

        // Return tankExplosionFX to pool
        tankExplosionFX.GetComponent<PoolableObject>().ReturnToPoolAfter(3f);
        OnDie?.Invoke();

        if (localTankInfo.Player.IsLocalPlayer)
        {
            Client.Singleton.SendToServer(new NetTDie(localTankInfo.Player.ID));

            Client.Singleton.SendToServer(new NetTKill(localTankInfo.Player.ID, damageDealerID));
        }
    }

    public void TakeDamage(float damage, byte damageDealerID, Vector3? damageSourcePos = null)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Die(damageDealerID);
        }
    }
}
