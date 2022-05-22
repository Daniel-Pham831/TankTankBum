using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

public class NetWelcome : NetMessage
{
    public SlotPlayerInformation MyPlayerInformation { set; get; }

    public byte TotalPlayer { set; get; }
    public List<SlotPlayerInformation> PlayerList { set; get; }


    public NetWelcome(SlotPlayerInformation player, byte totalPlayer, List<SlotPlayerInformation> playerList)
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

        SlotPlayerInformation.SerializeSlotPlayer(ref writer, MyPlayerInformation);

        writer.WriteByte(TotalPlayer);
        foreach (SlotPlayerInformation player in PlayerList)
        {
            SlotPlayerInformation.SerializeSlotPlayer(ref writer, player);
        }
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        MyPlayerInformation = SlotPlayerInformation.DeserializeSlotPlayer(ref reader);

        TotalPlayer = reader.ReadByte();
        PlayerList = new List<SlotPlayerInformation>();
        for (int i = 0; i < TotalPlayer; i++)
        {
            PlayerList.Add(SlotPlayerInformation.DeserializeSlotPlayer(ref reader));
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
