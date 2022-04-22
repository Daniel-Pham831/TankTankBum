using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetDisconnect : NetMessage
{
    public byte DisconnectedClientId { set; get; }

    public NetDisconnect(byte disconnectedClientId)
    {
        this.Code = OpCode.DISCONNECT;
        this.DisconnectedClientId = disconnectedClientId;
    }

    public NetDisconnect(ref DataStreamReader reader)
    {
        this.Code = OpCode.DISCONNECT;
        this.Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(this.DisconnectedClientId);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        this.DisconnectedClientId = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_DISCONNECT?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_DISCONNECT?.Invoke(this, cnn);
    }
}
