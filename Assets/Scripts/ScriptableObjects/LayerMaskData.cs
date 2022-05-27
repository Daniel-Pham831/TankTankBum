using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LayerMaskData", menuName = "TankTankBum/LayerMaskData", order = 0)]
public class LayerMaskData : SingletonScriptableObject<LayerMaskData>
{
    public LayerMask TankGrenade;
}
