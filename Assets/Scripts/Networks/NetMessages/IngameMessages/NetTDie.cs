using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTDie : NetMessage
{
    public byte ID { get; set; }
    public float NextSpawnDuration { get; set; }

    public NetTDie(byte id, float nextSpawnDuration = 0)
    {
        Code = OpCode.T_DIE;
        ID = id;
        NextSpawnDuration = nextSpawnDuration;
    }

    public NetTDie(ref DataStreamReader reader)
    {
        Code = OpCode.T_DIE;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFloat(NextSpawnDuration);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        NextSpawnDuration = reader.ReadFloat();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_DIE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_DIE?.Invoke(this, cnn);
    }
}
