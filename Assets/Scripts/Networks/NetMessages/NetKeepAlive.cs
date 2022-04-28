using Unity.Networking.Transport;

public class NetKeepAlive : NetMessage
{
    public NetKeepAlive()
    {
        Code = OpCode.KEEP_ALIVE;
    }

    public NetKeepAlive(ref DataStreamReader reader)
    {
        Code = OpCode.KEEP_ALIVE;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {

    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }
}
