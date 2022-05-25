using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetTKill : NetMessage
{
    public byte DeadPlayerID { get; set; }
    public byte KillerPlayerID { get; set; }

    public NetTKill(byte deadPlayerID, byte killerPlayerID)
    {
        Code = OpCode.T_KILL;
        DeadPlayerID = deadPlayerID;
        KillerPlayerID = killerPlayerID;
    }

    public NetTKill(ref DataStreamReader reader)
    {
        Code = OpCode.T_KILL;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(DeadPlayerID);
        writer.WriteByte(KillerPlayerID);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        DeadPlayerID = reader.ReadByte();
        KillerPlayerID = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_T_KILL?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_T_KILL?.Invoke(this, cnn);
    }
}

