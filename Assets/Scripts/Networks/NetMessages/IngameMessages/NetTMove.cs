using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTMove : NetMessage
{
    public byte ID { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public NetTMove(byte id, Vector3 position, Quaternion rotation)
    {
        this.Code = OpCode.TMOVE;
        this.ID = id;
        this.Position = position;
        this.Rotation = rotation;
    }

    public NetTMove(ref DataStreamReader reader)
    {
        this.Code = OpCode.TMOVE;
        this.Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(this.ID);
        writer.WriteFloat(this.Position.x);
        writer.WriteFloat(this.Position.y);
        writer.WriteFloat(this.Position.z);

        writer.WriteFloat(this.Rotation.x);
        writer.WriteFloat(this.Rotation.y);
        writer.WriteFloat(this.Rotation.z);
        writer.WriteFloat(this.Rotation.w);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        this.ID = reader.ReadByte();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        this.Position = new Vector3(x, y, z);

        x = reader.ReadFloat();
        y = reader.ReadFloat();
        z = reader.ReadFloat();
        float w = reader.ReadFloat();
        this.Rotation = new Quaternion(x, y, z, w);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_TMOVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_TMOVE?.Invoke(this, cnn);
    }
}
