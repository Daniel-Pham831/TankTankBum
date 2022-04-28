using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetDisconnect : NetMessage
{
    public byte DisconnectedClientId { set; get; }

    public NetDisconnect(byte disconnectedClientId)
    {
        Code = OpCode.DISCONNECT;

        DisconnectedClientId = disconnectedClientId;
    }

    public NetDisconnect(ref DataStreamReader reader)
    {
        Code = OpCode.DISCONNECT;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(DisconnectedClientId);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        DisconnectedClientId = reader.ReadByte();
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
