using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TankSpawnerData", menuName = "TankTankBum/TankSpawnerData", order = 0)]
public class TankSpawnerData : ScriptableObject
{
    [SerializeField] public GameObject tankPrefab;
    [SerializeField] public GameObject tankNamePrefab;
    [SerializeField] public GameObject tankHealthPrefab;
    [SerializeField] public GameObject localTankCameraPrefab;
    [SerializeField] public Material blueTankMaterial;
    [SerializeField] public Material redTankMaterial;
    [SerializeField] public float tankDefaultHealth = 100f;
}
