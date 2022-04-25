using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class NetWelcome : NetMessage
{
    public Player MyPlayerInformation { set; get; }

    public byte TotalPlayer { set; get; }
    public List<Player> PlayerList { set; get; }


    public NetWelcome(Player player, byte totalPlayer, List<Player> playerList)
    {
        this.Code = OpCode.WELCOME;

        this.MyPlayerInformation = player;

        this.TotalPlayer = totalPlayer;
        this.PlayerList = playerList;
    }

    public NetWelcome(ref DataStreamReader reader)
    {
        this.Code = OpCode.WELCOME;
        this.Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        Player.SerializePlayer(ref writer, this.MyPlayerInformation);

        writer.WriteByte(this.TotalPlayer);
        foreach (Player player in this.PlayerList)
        {
            Player.SerializePlayer(ref writer, player);
        }
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        this.MyPlayerInformation = Player.DeserializePlayer(ref reader);

        this.TotalPlayer = reader.ReadByte();
        this.PlayerList = new List<Player>();
        for (int i = 0; i < this.TotalPlayer; i++)
        {
            this.PlayerList.Add(Player.DeserializePlayer(ref reader));
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
