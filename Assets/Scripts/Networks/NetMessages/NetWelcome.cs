using Unity.Networking.Transport;

public class NetWelcome : NetMessage
{
    public NetWelcome()
    {
        this.Code = OpCode.WELCOME;
    }

    public NetWelcome(DataStreamReader reader)
    {
        this.Code = OpCode.WELCOME;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
    }

    public override void Deserialize(DataStreamReader reader)
    {
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_WELCOME?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
