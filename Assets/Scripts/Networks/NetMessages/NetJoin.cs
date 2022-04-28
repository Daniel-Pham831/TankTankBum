using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetJoin : NetMessage
{
    public Player JoinedPlayer { set; get; }

    public NetJoin(Player joinedPlayer)
    {
        Code = OpCode.JOIN;
        JoinedPlayer = joinedPlayer;
    }

    public NetJoin(ref DataStreamReader reader)
    {
        Code = OpCode.JOIN;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        Player.SerializePlayer(ref writer, JoinedPlayer);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        JoinedPlayer = Player.DeserializePlayer(ref reader);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_JOIN?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_JOIN?.Invoke(this, cnn);
    }
}
