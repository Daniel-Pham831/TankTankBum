using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

public enum Team
{
    Blue = 0,
    Red = 1
}

public enum ReadyState
{
    Ready = 0,
    Unready = 1
}

public class Player
{
    public byte Id;
    public Team Team;
    public byte SlotIndex;
    public string Name;
    public ReadyState ReadyState;

    public Player()
    {
        this.Id = 0;
        this.Team = 0;
        this.SlotIndex = 0;
        this.Name = "";
        this.ReadyState = ReadyState.Unready;
    }

    public Player(byte id, Team team, byte slotIndex, string name, ReadyState readyState)
    {
        this.Id = id;
        this.Team = team;
        this.SlotIndex = slotIndex;
        this.Name = name;
        this.ReadyState = readyState;
    }

    public static void SerializePlayer(ref DataStreamWriter writer, Player player)
    {
        writer.WriteByte(player.Id);
        writer.WriteByte((byte)player.Team);
        writer.WriteByte(player.SlotIndex);
        writer.WriteFixedString32(player.Name);
        writer.WriteByte((byte)player.ReadyState);
    }

    public static Player DeserializePlayer(ref DataStreamReader reader)
    {
        byte playerId = reader.ReadByte();
        Team playerTeam = (Team)reader.ReadByte();
        byte playerSlotIndex = reader.ReadByte();
        string playerName = reader.ReadFixedString32().ToString();
        ReadyState readyState = (ReadyState)reader.ReadByte();

        return new Player(playerId, playerTeam, playerSlotIndex, playerName, readyState);
    }

    public static Player FindPlayerWithID(List<Player> playerList, byte playerId)
    {
        foreach (Player player in playerList)
        {
            if (player.Id == playerId) return player;
        }
        return null;
    }

    public static Player FindPlayerWithIDAndRemove(ref List<Player> playerList, byte playerId)
    {
        foreach (Player player in playerList)
        {
            if (player.Id == playerId)
            {
                playerList.Remove(player);
                return player;
            }
        }
        return null;
    }

    public static bool HaveAllPlayersReadied(List<Player> players)
    {
        foreach (Player player in players)
        {
            if (player.ReadyState == ReadyState.Unready)
                return false;
        }
        return true;
    }

    public void SwitchTeam()
    {
        this.Team = this.Team == Team.Blue ? Team.Red : Team.Blue;
    }

    public void SwitchReadyState()
    {
        this.ReadyState = this.ReadyState == ReadyState.Ready ? ReadyState.Unready : ReadyState.Ready;
    }

    public bool IsHost => this.Id == GameInformation.Singleton.HostId;
}
