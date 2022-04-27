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
        this.Code = OpCode.T_INPUT;
        this.ID = id;
        this.HorizontalInput = horizontalInput;
        this.VerticalInput = verticalInput;
    }

    public NetTInput(ref DataStreamReader reader)
    {
        this.Code = OpCode.T_INPUT;
        this.Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(this.ID);
        writer.WriteFloat(this.HorizontalInput);
        writer.WriteFloat(this.VerticalInput);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        this.ID = reader.ReadByte();
        this.HorizontalInput = reader.ReadFloat();
        this.VerticalInput = reader.ReadFloat();
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
