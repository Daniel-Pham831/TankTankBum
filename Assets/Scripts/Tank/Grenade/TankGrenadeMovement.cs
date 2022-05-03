using System.Collections;
using System.Collections.Generic;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TankInformation>(out TankInformation tankInformation))
        {

            if (tankInformation.ID == grenadeInformation.ID)
                return;
        }

        poolableObject.ReleaseAction?.Invoke(this.gameObject);
    }
}
