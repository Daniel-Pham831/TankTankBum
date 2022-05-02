using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTFireInput : NetMessage
{
    public byte ID { get; set; }
    public Vector3 FireDirection { get; set; }

    public NetTFireInput(byte id, Vector3 fireDirection)
    {
        Code = OpCode.T_FIRE_INPUT;
        ID = id;
        FireDirection = fireDirection;
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

        writer.WriteFloat(FireDirection.x);
        writer.WriteFloat(FireDirection.y);
        writer.WriteFloat(FireDirection.z);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();

        FireDirection = new Vector3(x, y, z);
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
