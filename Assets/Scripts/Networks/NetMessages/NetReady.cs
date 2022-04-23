using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetReady : NetMessage
{
    public Player Player { set; get; }
    public NetReady(Player player)
    {
        this.Code = OpCode.READY;
        this.Player = player;
    }

    public NetReady(ref DataStreamReader reader)
    {
        this.Code = OpCode.READY;
        this.Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        Player.SerializePlayer(ref writer, this.Player);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        this.Player = Player.DeserializePlayer(ref reader);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_READY?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_READY?.Invoke(this, cnn);
    }
}
