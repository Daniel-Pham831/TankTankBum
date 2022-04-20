using System.Collections.Generic;
using Unity.Networking.Transport;

public class NetWelcome : NetMessage
{
    public byte AssignedId { set; get; }
    public byte TotalPlayer { set; get; }
    public List<Player> PlayerList { set; get; }

    public NetWelcome(byte id, byte totalPlayer, List<Player> playerList)
    {
        this.Code = OpCode.WELCOME;

        this.AssignedId = id;
        this.TotalPlayer = totalPlayer;
        this.PlayerList = playerList;
    }

    public NetWelcome(DataStreamReader reader)
    {
        this.Code = OpCode.WELCOME;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(this.AssignedId);

        writer.WriteByte(this.TotalPlayer);
        foreach (Player player in this.PlayerList)
        {
            writer.WriteByte(player.Id);
            writer.WriteFixedString32(player.Name);
        }
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.AssignedId = reader.ReadByte();

        this.TotalPlayer = reader.ReadByte();
        this.PlayerList = new List<Player>();
        for (int i = 0; i < this.TotalPlayer; i++)
        {
            byte playerId = reader.ReadByte();
            string playerName = reader.ReadFixedString32().ToString();

            this.PlayerList.Add(new Player(playerId, playerName));
        }
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_WELCOME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
