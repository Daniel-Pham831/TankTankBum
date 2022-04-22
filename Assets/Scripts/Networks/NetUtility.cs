using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    SEND_NAME = 2,
    JOIN = 3,
    WELCOME = 4,
}

public static class NetUtility
{
    // Net messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_SEND_NAME;
    public static Action<NetMessage> C_JOIN;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_SEND_NAME;
    public static Action<NetMessage, NetworkConnection> S_JOIN;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;

    public static void OnData(ref DataStreamReader streamReader, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var OpCode = (OpCode)streamReader.ReadByte();
        switch (OpCode)
        {
            case OpCode.KEEP_ALIVE:
                msg = new NetKeepAlive(ref streamReader);
                break;

            case OpCode.SEND_NAME:
                msg = new NetSendName(ref streamReader);
                break;

            case OpCode.JOIN:
                msg = new NetJoin(ref streamReader);
                break;

            case OpCode.WELCOME:
                msg = new NetWelcome(ref streamReader);
                break;

            default:
                Debug.LogError("Message received has no OpCode");
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else
            msg.ReceivedOnClient();
    }
}
