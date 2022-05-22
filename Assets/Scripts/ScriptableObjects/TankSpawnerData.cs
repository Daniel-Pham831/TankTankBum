using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TankSpawnerData", menuName = "TankTankBum/TankSpawnerData", order = 0)]
public class TankSpawnerData : ScriptableObject
{
    [SerializeField] public GameObject TankPrefab;
    [SerializeField] public GameObject TankNamePrefab;
    [SerializeField] public GameObject TankHealthPrefab;
    [SerializeField] public GameObject LocalTankCameraPrefab;
    [SerializeField] public Material BlueTankMaterial;
    [SerializeField] public Material RedTankMaterial;
    [SerializeField] public float TankDefaultHealth = 100f;

    [SerializeField] private float attackRespawnTime = 3f;
    [SerializeField] private float defendRespawnTime = 3f;
    [SerializeField] private int attackMaxSpawnLives = 5;
    [SerializeField] private int defendMaxSpawnLives = 5;

    public float GetRespawnTime(Role role) => role == Role.Attacker ? attackRespawnTime : defendRespawnTime;
    public int GetMaxSpawnLives(Role role) => role == Role.Attacker ? attackMaxSpawnLives : defendMaxSpawnLives;
}
