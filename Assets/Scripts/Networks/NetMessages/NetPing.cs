using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetPing : NetMessage
{
    public NetPing()
    {
        Code = OpCode.PING;
    }

    public NetPing(ref DataStreamReader reader)
    {
        Code = OpCode.PING;
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

        NetUtility.C_PING?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_PING?.Invoke(this, cnn);
    }
}

