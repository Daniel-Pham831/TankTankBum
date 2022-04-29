using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTInput : NetMessage
{
    public byte ID { get; set; }
    public float HorizontalInput { get; set; }
    public float VerticalInput { get; set; }

    public NetTInput(byte id, float horizontalInput, float verticalInput)
    {
        Code = OpCode.T_INPUT;
        ID = id;
        HorizontalInput = horizontalInput;
        VerticalInput = verticalInput;
    }

    public NetTInput(ref DataStreamReader reader)
    {
        Code = OpCode.T_INPUT;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteFloat(HorizontalInput);
        writer.WriteFloat(VerticalInput);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        HorizontalInput = reader.ReadFloat();
        VerticalInput = reader.ReadFloat();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_INPUT?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_INPUT?.Invoke(this, cnn);
    }
}
