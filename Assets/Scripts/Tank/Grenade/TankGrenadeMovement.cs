using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TankGrenadeMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GrenadeInformation grenadeInformation;
    [SerializeField] private PoolableObject poolableObject;

    public void FireAtDirection(Vector3 direction, Vector3 position, float speed)
    {
        transform.forward = direction;
        transform.position = position;
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

        this.GetComponentInChildren<TrailRenderer>().Clear();

        poolableObject.ReturnToPoolAfter(10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TankInformation>(out TankInformation tankInformation))
        {

            if (tankInformation.ID == grenadeInformation.ID)
                return;
        }

        Pool GrenadeExplosionPool = ObjectPoolManager.Singleton.GetPool(PoolType.GrenadeExplosion);
        GameObject grenadeExplosion = GrenadeExplosionPool.Get();
        grenadeExplosion.transform.position = transform.position;

        grenadeExplosion.GetComponent<PoolableObject>()?.ReturnToPoolAfter(5);
        if (grenadeExplosion.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
        {
            ps.Play();
        }

        poolableObject.ReleaseAction?.Invoke(this.gameObject);
    }

}
