using UnityEngine;

public interface IDamageable : IDieable
{
    public float Health { get; set; }
    public void TakeDamage(float damage, byte damageDealerID, Vector3? damageSourcePos = null);
}
