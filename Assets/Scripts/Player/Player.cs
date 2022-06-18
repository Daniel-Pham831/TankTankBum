using Unity.Networking.Transport;
using UnityEngine;

// This class is for storing a player data
public class Player
{
    public byte ID;
    public Team Team;
    public string Name;
    public bool IsLocalPlayer;
    public bool IsHost;
    public Role Role => RoleController.Singleton.GetRole(Team);

    public Player() { }

    public Player(byte id, Team team, string name, bool isLocalPlayer, bool isHost)
    {
        ID = id;
        Team = team;
        Name = name;
        IsLocalPlayer = isLocalPlayer;
        IsHost = isHost;
    }

    public static void SerializePlayer(ref DataStreamWriter writer, Player player)
    {
        writer.WriteByte(player.ID);
        writer.WriteByte((byte)player.Team);
        writer.WriteFixedString32(player.Name);
        writer.WriteFixedString32(player.IsLocalPlayer.ToString());
        writer.WriteFixedString32(player.IsHost.ToString());
    }

    public static Player DeserializePlayer(ref DataStreamReader reader)
    {
        string trueValue = true.ToString(); // For compare

        byte id = reader.ReadByte();
        Team team = (Team)reader.ReadByte();
        string name = reader.ReadFixedString32().ToString();
        bool isLocalPlayer = reader.ReadFixedString32().ToString() == trueValue;
        bool isHost = reader.ReadFixedString32().ToString() == trueValue;

        return new Player(id, team, name, isLocalPlayer, isHost);
    }
}
