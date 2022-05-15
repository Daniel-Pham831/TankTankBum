using System;
using UnityEngine;

public class TankGrenadeMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GrenadeInformation grenadeInformation;
    [SerializeField] private GrenadeExplosionInformation grenadeExplosionInformation;
    [SerializeField] private PoolableObject poolableObject;
    private float timeReturnGrenadeToPool = 10f;

    public void FireAtDirection(Vector3 direction, Vector3 position, float speed)
    {
        transform.forward = direction;
        transform.position = position;
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

        this.GetComponentInChildren<TrailRenderer>().Clear();

        //Return the grenade to pool if it doesn't collide with any colliders
        poolableObject.ReturnToPoolAfter(timeReturnGrenadeToPool);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Prevent the grenade to collide with the tank that shot it
        if (other.TryGetComponent<TankInformation>(out TankInformation tankInformation))
            if (tankInformation.ID == grenadeInformation.ID)
                return;

        ApplyDamageToAll();

        SpawnExplosionFXOnTrigger();

        // Return this Grenade to its pool
        poolableObject.ReleaseAction?.Invoke(this.gameObject);
    }

    private void ApplyDamageToAll()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, grenadeExplosionInformation.ExplosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                float damage = grenadeExplosionInformation.Damage;
                if (collider.TryGetComponent<TankInformation>(out TankInformation tankInformation))
                {
                    damage = grenadeInformation.Team != tankInformation.Team ? damage : grenadeExplosionInformation.SameTeamDamage;
                }

                damageable.TakeDamage(damage);
            }
        }
    }

    private void SpawnExplosionFXOnTrigger()
    {
        Pool GrenadeExplosionPool = ObjectPoolManager.Singleton.GetPool(PoolType.GrenadeExplosion);
        GameObject grenadeExplosion = GrenadeExplosionPool.Get();
        grenadeExplosion.transform.position = transform.position;

        grenadeExplosion.GetComponent<PoolableObject>()?.ReturnToPoolAfter(2);
        if (grenadeExplosion.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            ps.Play();
    }
}
