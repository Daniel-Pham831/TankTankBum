using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTRotation : NetMessage
{
    public byte ID { get; set; }
    public Vector3 Forward { get; set; }

    public NetTRotation(byte id, Vector3 forward)
    {
        Code = OpCode.T_ROTATION;
        ID = id;
        Forward = forward;
    }

    public NetTRotation(ref DataStreamReader reader)
    {
        Code = OpCode.T_ROTATION;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFloat(Forward.x);
        writer.WriteFloat(Forward.y);
        writer.WriteFloat(Forward.z);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        Forward = new Vector3(x, y, z);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_ROTATION?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_ROTATION?.Invoke(this, cnn);
    }
}
