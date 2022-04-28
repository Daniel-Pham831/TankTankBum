using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetReady : NetMessage
{
    public byte Id { set; get; }
    public NetReady(byte playerId)
    {
        Code = OpCode.READY;
        Id = playerId;
    }

    public NetReady(ref DataStreamReader reader)
    {
        Code = OpCode.READY;
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

        NetUtility.C_READY?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_READY?.Invoke(this, cnn);
    }
}
