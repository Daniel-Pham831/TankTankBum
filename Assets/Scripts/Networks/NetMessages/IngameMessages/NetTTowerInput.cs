using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTTowerInput : NetMessage
{
    public byte ID { get; set; }
    public float RotationInput { get; set; }

    public NetTTowerInput(byte id, float rotationInput)
    {
        Code = OpCode.T_TOWER_INPUT;
        ID = id;
        RotationInput = rotationInput;
    }

    public NetTTowerInput(ref DataStreamReader reader)
    {
        Code = OpCode.T_TOWER_INPUT;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFloat(RotationInput);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        RotationInput = reader.ReadFloat();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_TOWER_INPUT?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_TOWER_INPUT?.Invoke(this, cnn);
    }
}