using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetSwitchTeam : NetMessage
{
    public byte Id { set; get; }

    public NetSwitchTeam(byte id)
    {
        Code = OpCode.SWITCHTEAM;
        Id = id;
    }

    public NetSwitchTeam(ref DataStreamReader reader)
    {
        Code = OpCode.SWITCHTEAM;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(Id);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        Id = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_SWITCHTEAM?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_SWITCHTEAM?.Invoke(this, cnn);
    }
}
