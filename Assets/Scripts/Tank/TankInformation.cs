using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Role
{
    Attacker = 0,
    Defender = 1
}

/*
    This class is for storing ONE TankInformation
*/
public class TankInformation : MonoBehaviour
{
    public byte ID;
    public Team Team;
    public bool IsLocalPlayer;
    public bool IsHost;

    public void Setup(byte id, Team team, bool isLocalPlayer, bool isHost)
    {
        ID = id;
        Team = team;
        IsLocalPlayer = isLocalPlayer;
        IsHost = isHost;
    }
}
