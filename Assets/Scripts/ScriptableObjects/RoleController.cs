using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Role
{
    Attacker = 0,
    Defender = 1
}

public class RoleController : MonoBehaviour
{
    public static RoleController Singleton { get; private set; }
    public Team currentAttackerTeam;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    public Role GetRole(Team team)
    {
        return team == currentAttackerTeam ? Role.Attacker : Role.Defender;
    }

    public void SwitchRole()
    {
        currentAttackerTeam = currentAttackerTeam == Team.Blue ? Team.Red : Team.Blue;
    }
}