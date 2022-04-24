using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetSwitchTeam : NetMessage
{
    public byte Id { set; get; }

    public NetSwitchTeam(byte Id)
    {
        this.Code = OpCode.SWITCHTEAM;
        this.Id = Id;
    }

    public NetSwitchTeam(ref DataStreamReader reader)
    {
        this.Code = OpCode.SWITCHTEAM;
        this.Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(this.Id);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        this.Id = reader.ReadByte();
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
