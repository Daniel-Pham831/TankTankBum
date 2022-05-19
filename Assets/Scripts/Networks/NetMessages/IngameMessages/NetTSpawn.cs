using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTSpawn : NetMessage
{
    public Player Player { get; set; }

    public NetTSpawn(Player player)
    {
        Code = OpCode.T_SPAWN;
        Player = player;
    }

    public NetTSpawn(ref DataStreamReader reader)
    {
        Code = OpCode.T_SPAWN;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        Player.SerializePlayer(ref writer, Player);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        Player = Player.DeserializePlayer(ref reader);
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_SPAWN?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_SPAWN?.Invoke(this, cnn);
    }
}
