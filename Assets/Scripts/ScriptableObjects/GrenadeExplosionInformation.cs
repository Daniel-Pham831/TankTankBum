using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeExplosionInformation", menuName = "TankTankBum/GrenadeExplosionInformation", order = 0)]
public class GrenadeExplosionInformation : ScriptableObject
{
    public float ExplosionRadius;
    public float Damage;
    public float SameTeamDamage;
}
