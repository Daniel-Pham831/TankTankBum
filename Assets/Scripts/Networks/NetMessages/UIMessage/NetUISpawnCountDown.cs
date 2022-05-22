using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetUISpawnCountDown : NetMessage
{
    public byte ID { get; set; }
    public byte CountDownDuration { get; set; }

    public NetUISpawnCountDown(byte id, byte countDownDuration)
    {
        Code = OpCode.UI_SPAWN_COUNTDOWN;
        ID = id;
        CountDownDuration = countDownDuration;
    }

    public NetUISpawnCountDown(ref DataStreamReader reader)
    {
        Code = OpCode.UI_SPAWN_COUNTDOWN;
        Deserialize(ref reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteByte(ID);
        writer.WriteByte(CountDownDuration);
    }

    public override void Deserialize(ref DataStreamReader reader)
    {
        ID = reader.ReadByte();
        CountDownDuration = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_UI_SPAWN_COUNTDOWN?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_UI_SPAWN_COUNTDOWN?.Invoke(this, cnn);
    }
}
