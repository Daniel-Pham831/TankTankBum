using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    SEND_NAME = 2,
    JOIN = 3,
    DISCONNECT = 4,
    WELCOME = 5,
    READY = 6,
    SWITCHTEAM = 7,
    START = 8,
    TMOVE = 9
}

public static class NetUtility
{
    // Net Lobby messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_SEND_NAME;
    public static Action<NetMessage> C_JOIN;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_DISCONNECT;
    public static Action<NetMessage> C_READY;
    public static Action<NetMessage> C_SWITCHTEAM;
    public static Action<NetMessage> C_START;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_SEND_NAME;
    public static Action<NetMessage, NetworkConnection> S_JOIN;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_DISCONNECT;
    public static Action<NetMessage, NetworkConnection> S_READY;
    public static Action<NetMessage, NetworkConnection> S_SWITCHTEAM;
    public static Action<NetMessage, NetworkConnection> S_START;

    // Net InGame messages
    public static Action<NetMessage> C_TMOVE;
    public static Action<NetMessage, NetworkConnection> S_TMOVE;


    public static void OnData(ref DataStreamReader streamReader, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var OpCode = (OpCode)streamReader.ReadByte();
        switch (OpCode)
        {
            case OpCode.KEEP_ALIVE:
                msg = new NetKeepAlive(ref streamReader);
                break;

            //Lobby
            case OpCode.SEND_NAME:
                msg = new NetSendName(ref streamReader);
                break;

            case OpCode.JOIN:
                msg = new NetJoin(ref streamReader);
                break;

            case OpCode.DISCONNECT:
                msg = new NetDisconnect(ref streamReader);
                break;

            case OpCode.WELCOME:
                msg = new NetWelcome(ref streamReader);
                break;

            case OpCode.READY:
                msg = new NetReady(ref streamReader);
                break;

            case OpCode.SWITCHTEAM:
                msg = new NetSwitchTeam(ref streamReader);
                break;

            case OpCode.START:
                msg = new NetStartGame(ref streamReader);
                break;

            //InGame
            case OpCode.TMOVE:
                msg = new NetTMove(ref streamReader);
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
