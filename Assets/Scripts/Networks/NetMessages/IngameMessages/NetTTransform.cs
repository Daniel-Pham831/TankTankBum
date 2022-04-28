using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTTransform : NetMessage
{
    public byte ID { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public NetTTransform(byte id, Vector3 position, Quaternion rotation)
    {
        Code = OpCode.T_TRANSFORM;
        ID = id;
        Position = position;
        Rotation = rotation;
    }

    public NetTTransform(ref DataStreamReader reader)
    {
        Code = OpCode.T_TRANSFORM;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFloat(Position.x);
        writer.WriteFloat(Position.y);
        writer.WriteFloat(Position.z);

        writer.WriteFloat(Rotation.x);
        writer.WriteFloat(Rotation.y);
        writer.WriteFloat(Rotation.z);
        writer.WriteFloat(Rotation.w);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        Position = new Vector3(x, y, z);

        x = reader.ReadFloat();
        y = reader.ReadFloat();
        z = reader.ReadFloat();
        float w = reader.ReadFloat();
        Rotation = new Quaternion(x, y, z, w);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_TRANSFORM?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_TRANSFORM?.Invoke(this, cnn);
    }
}
