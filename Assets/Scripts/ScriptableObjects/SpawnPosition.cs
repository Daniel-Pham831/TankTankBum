using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPosition", menuName = "TankTankBum/SpawnPosition", order = 0)]
public class SpawnPosition : ScriptableObject
{
    [SerializeField] private Transform[] attackerPositions;
    [SerializeField] private Transform[] defenderPositions;
    private int attackCounter = 0;
    private int defendCounter = 0;

    public Transform GetPosition(Role role)
    {
        return role == Role.Attacker ?
        attackerPositions[attackCounter++ % attackerPositions.Length] :
        defenderPositions[defendCounter++ % defenderPositions.Length];
    }
}

