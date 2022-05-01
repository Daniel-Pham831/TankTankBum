using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTPosition : NetMessage
{
    public byte ID { get; set; }
    public Vector3 Position { get; set; }

    public NetTPosition(byte id, Vector3 position)
    {
        Code = OpCode.T_POSITION;
        ID = id;
        Position = position;
    }

    public NetTPosition(ref DataStreamReader reader)
    {
        Code = OpCode.T_POSITION;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFloat(Position.x);
        writer.WriteFloat(Position.y);
        writer.WriteFloat(Position.z);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        Position = new Vector3(x, y, z);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_POSITION?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_POSITION?.Invoke(this, cnn);
    }
}

