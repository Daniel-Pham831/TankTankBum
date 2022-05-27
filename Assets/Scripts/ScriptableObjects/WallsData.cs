using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallsData", menuName = "TankTankBum/WallsData", order = 0)]
public class WallsData : SingletonScriptableObject<WallsData>
{
    public float defaultWallHealth = 100;
    public float EffectRadius = 1.25f;
}
