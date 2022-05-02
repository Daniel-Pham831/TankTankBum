using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTFireInput : NetMessage
{
    public byte ID { get; set; }

    public NetTFireInput(byte id)
    {
        Code = OpCode.T_FIRE_INPUT;
        ID = id;
    }

    public NetTFireInput(ref DataStreamReader reader)
    {
        Code = OpCode.T_FIRE_INPUT;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_FIRE_INPUT?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_FIRE_INPUT?.Invoke(this, cnn);
    }
}
