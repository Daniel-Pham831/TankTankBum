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
        Code = OpCode.WELCOME;

        MyPlayerInformation = player;

        TotalPlayer = totalPlayer;
        PlayerList = playerList;
    }

    public NetWelcome(ref DataStreamReader reader)
    {
        Code = OpCode.WELCOME;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        Player.SerializePlayer(ref writer, MyPlayerInformation);

        writer.WriteByte(TotalPlayer);
        foreach (Player player in PlayerList)
        {
            Player.SerializePlayer(ref writer, player);
        }
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        MyPlayerInformation = Player.DeserializePlayer(ref reader);

        TotalPlayer = reader.ReadByte();
        PlayerList = new List<Player>();
        for (int i = 0; i < TotalPlayer; i++)
        {
            PlayerList.Add(Player.DeserializePlayer(ref reader));
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
