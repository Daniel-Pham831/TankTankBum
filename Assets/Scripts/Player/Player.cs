using UnityEngine;

// This class is for storing a player data
public class Player
{
    public byte ID;
    public Team Team;
    public string Name;
    public bool IsLocalPlayer;
    public bool IsHost;

    public Player(byte id, Team team, string name, bool isLocalPlayer, bool isHost)
    {
        ID = id;
        Team = team;
        Name = name;
        IsLocalPlayer = isLocalPlayer;
        IsHost = isHost;
    }
}
