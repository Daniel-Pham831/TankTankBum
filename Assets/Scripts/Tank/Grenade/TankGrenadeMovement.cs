using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TankGrenadeMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GrenadeInformation grenadeInformation;
    [SerializeField] private PoolableObject poolableObject;
    private Coroutine returnToPool;

    public void FireAtDirection(Vector3 direction, Vector3 position, float speed)
    {
        transform.forward = direction;
        transform.position = position;
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

        this.GetComponentInChildren<TrailRenderer>().Clear();

        returnToPool = StartCoroutine(ReturnToPoolAfter(10));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TankInformation>(out TankInformation tankInformation))
        {

            if (tankInformation.ID == grenadeInformation.ID)
                return;
        }

        if (returnToPool != null)
            StopCoroutine(returnToPool);

        poolableObject.ReleaseAction?.Invoke(this.gameObject);
    }

    IEnumerator ReturnToPoolAfter(float second)
    {
        yield return new WaitForSeconds(second);
        poolableObject.ReleaseAction?.Invoke(this.gameObject);
    }
}
