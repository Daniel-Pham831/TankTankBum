using System.Collections.Generic;
using Unity.Networking.Transport;

public class NetSendName : NetMessage
{
    public string Name { set; get; }

    public NetSendName(string name)
    {
        this.Code = OpCode.SEND_NAME;
        this.Name = name;
    }

    public NetSendName(DataStreamReader reader)
    {
        this.Code = OpCode.SEND_NAME;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteFixedString32(this.Name);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.Name = reader.ReadFixedString32().ToString();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_SEND_NAME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_SEND_NAME?.Invoke(this, cnn);
    }
}
