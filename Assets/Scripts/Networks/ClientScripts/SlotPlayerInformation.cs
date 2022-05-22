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

/// <summary> This class is for storing a player information of a specific slot</summary>
public class SlotPlayerInformation
{
    public byte Id;
    public Team Team;
    public byte SlotIndex;
    public string Name;
    public ReadyState ReadyState;

    public SlotPlayerInformation()
    {
        Id = 0;
        Team = 0;
        SlotIndex = 0;
        Name = "";
        ReadyState = ReadyState.Unready;
    }

    public SlotPlayerInformation(byte id, Team team, byte slotIndex, string name, ReadyState readyState)
    {
        Id = id;
        Team = team;
        SlotIndex = slotIndex;
        Name = name;
        ReadyState = readyState;
    }

    public static void SerializeSlotPlayer(ref DataStreamWriter writer, SlotPlayerInformation slotPlayer)
    {
        writer.WriteByte(slotPlayer.Id);
        writer.WriteByte((byte)slotPlayer.Team);
        writer.WriteByte(slotPlayer.SlotIndex);
        writer.WriteFixedString32(slotPlayer.Name);
        writer.WriteByte((byte)slotPlayer.ReadyState);
    }

    public static SlotPlayerInformation DeserializeSlotPlayer(ref DataStreamReader reader)
    {
        byte playerId = reader.ReadByte();
        Team playerTeam = (Team)reader.ReadByte();
        byte playerSlotIndex = reader.ReadByte();
        string playerName = reader.ReadFixedString32().ToString();
        ReadyState readyState = (ReadyState)reader.ReadByte();

        return new SlotPlayerInformation(playerId, playerTeam, playerSlotIndex, playerName, readyState);
    }

    public static SlotPlayerInformation FindSlotPlayerWithID(List<SlotPlayerInformation> playerList, byte playerId)
    {
        foreach (SlotPlayerInformation player in playerList)
        {
            if (player.Id == playerId) return player;
        }
        return null;
    }

    public static SlotPlayerInformation FindSlotPlayerWithIDAndRemove(ref List<SlotPlayerInformation> playerList, byte playerId)
    {
        foreach (SlotPlayerInformation player in playerList)
        {
            if (player.Id == playerId)
            {
                playerList.Remove(player);
                return player;
            }
        }
        return null;
    }

    public static bool HaveAllPlayersReadied(List<SlotPlayerInformation> players)
    {
        foreach (SlotPlayerInformation player in players)
        {
            if (player.ReadyState == ReadyState.Unready)
                return false;
        }
        return true;
    }

    public static bool Have2TeamsEqual(List<SlotPlayerInformation> players)
    {
        return MathF.Abs(CountTeamPlayer(players, Team.Blue) - CountTeamPlayer(players, Team.Red)) <= 1;
    }

    public static int CountTeamPlayer(List<SlotPlayerInformation> players, Team team)
    {
        int counter = 0;
        foreach (SlotPlayerInformation player in players)
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
