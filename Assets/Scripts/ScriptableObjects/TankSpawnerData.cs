using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TankSpawnerData", menuName = "TankTankBum/TankSpawnerData", order = 0)]
public class TankSpawnerData : ScriptableObject
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject tankNamePrefab;
    [SerializeField] private GameObject tankHealthPrefab;
    [SerializeField] private GameObject localTankCameraPrefab;
    [SerializeField] private Material blueTankMaterial;
    [SerializeField] private Material redTankMaterial;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private float tankDefaultHealth = 100f;
}
