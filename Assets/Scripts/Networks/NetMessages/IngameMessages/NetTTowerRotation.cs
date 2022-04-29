using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTTowerRotation : NetMessage
{
    public byte ID { get; set; }
    public Quaternion Rotation { get; set; }

    public NetTTowerRotation(byte id, Quaternion rotation)
    {
        Code = OpCode.T_TOWER_ROTATION;
        ID = id;
        Rotation = rotation;
    }

    public NetTTowerRotation(ref DataStreamReader reader)
    {
        Code = OpCode.T_TOWER_ROTATION;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);

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
        float w = reader.ReadFloat();
        Rotation = new Quaternion(x, y, z, w);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_TOWER_ROTATION?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_TOWER_ROTATION?.Invoke(this, cnn);
    }
}
