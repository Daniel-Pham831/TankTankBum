using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetStartGame : NetMessage
{
    public NetStartGame()
    {
        Code = OpCode.START;
    }

    public NetStartGame(ref DataStreamReader reader)
    {
        Code = OpCode.START;
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

        NetUtility.C_START?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_START?.Invoke(this, cnn);
    }
}
