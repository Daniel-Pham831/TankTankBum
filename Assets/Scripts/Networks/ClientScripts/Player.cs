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
        Id = 0;
        Team = 0;
        SlotIndex = 0;
        Name = "";
        ReadyState = ReadyState.Unready;
    }

    public Player(byte id, Team team, byte slotIndex, string name, ReadyState readyState)
    {
        Id = id;
        Team = team;
        SlotIndex = slotIndex;
        Name = name;
        ReadyState = readyState;
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

    public static bool Have2TeamsEqual(List<Player> players)
    {
        return MathF.Abs(CountTeamPlayer(players, Team.Blue) - CountTeamPlayer(players, Team.Red)) <= 1;
    }

    public static int CountTeamPlayer(List<Player> players, Team team)
    {
        int counter = 0;
        foreach (Player player in players)
        {
            if (player.Team == team)
                counter++;
        }
        return counter;
    }

    public void SwitchTeam()
    {
        Team = Team == Team.Blue ? Team.Red : Team.Blue;
    }

    public void SwitchReadyState()
    {
        ReadyState = ReadyState == ReadyState.Ready ? ReadyState.Unready : ReadyState.Ready;
    }

    public bool IsHost => Id == GameInformation.Singleton.HostId;
}
