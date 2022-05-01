using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTTowerRotation : NetMessage
{
    public byte ID { get; set; }
    public Vector3 LocalEulerAngles { get; set; }

    public NetTTowerRotation(byte id, Vector3 localEulerAngles)
    {
        Code = OpCode.T_TOWER_ROTATION;
        ID = id;
        LocalEulerAngles = localEulerAngles;
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

        writer.WriteFloat(LocalEulerAngles.x);
        writer.WriteFloat(LocalEulerAngles.y);
        writer.WriteFloat(LocalEulerAngles.z);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        LocalEulerAngles = new Vector3(x, y, z);
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
