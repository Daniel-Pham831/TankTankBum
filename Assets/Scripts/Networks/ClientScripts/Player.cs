using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

public enum Team
{
    Blue = 0,
    Red = 1
}


public class Player
{
    public byte Id;
    public Team Team;
    public byte SlotIndex;
    public string Name;

    public Player()
    {
        this.Id = 0;
        this.Team = 0;
        this.SlotIndex = 0;
        this.Name = "";
    }

    public Player(byte id, Team team, byte slotIndex, string name)
    {
        this.Id = id;
        this.Team = team;
        this.SlotIndex = slotIndex;
        this.Name = name;
    }

    public static void SerializePlayer(ref DataStreamWriter writer, Player player)
    {
        writer.WriteByte(player.Id);
        writer.WriteByte((byte)player.Team);
        writer.WriteByte(player.SlotIndex);
        writer.WriteFixedString32(player.Name);
    }

    public static Player DeserializePlayer(ref DataStreamReader reader)
    {
        byte playerId = reader.ReadByte();
        Team playerTeam = (Team)reader.ReadByte();
        byte playerSlotIndex = reader.ReadByte();
        string playerName = reader.ReadFixedString32().ToString();

        return new Player(playerId, playerTeam, playerSlotIndex, playerName);
    }

    public static Player FindPlayer(ref List<Player> playerList, byte disconnectedClientId)
    {
        foreach (Player player in playerList)
        {
            if (player.Id == disconnectedClientId)
                return player;
        }

        return null;
    }
}
