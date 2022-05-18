using UnityEngine;

// This class is for storing a player data
public class Player
{
    public byte ID;
    public Team Team;
    public string Name;

    public Player(byte id, Team team, string name)
    {
        ID = id;
        Team = team;
        Name = name;
    }
}
