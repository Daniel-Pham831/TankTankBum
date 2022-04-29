using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    PING = 0,
    KEEP_ALIVE = 1,
    SEND_NAME = 2,
    JOIN = 3,
    DISCONNECT = 4,
    WELCOME = 5,
    READY = 6,
    SWITCHTEAM = 7,
    START = 8,
    T_TRANSFORM = 9,
    T_INPUT = 10,
    T_TOWER_INPUT = 11,
    T_TOWER_ROTATION = 12
}

public static class NetUtility
{
    // Net Lobby messages
    public static Action<NetMessage> C_PING;
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_SEND_NAME;
    public static Action<NetMessage> C_JOIN;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_DISCONNECT;
    public static Action<NetMessage> C_READY;
    public static Action<NetMessage> C_SWITCHTEAM;
    public static Action<NetMessage> C_START;
    public static Action<NetMessage, NetworkConnection> S_PING;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_SEND_NAME;
    public static Action<NetMessage, NetworkConnection> S_JOIN;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_DISCONNECT;
    public static Action<NetMessage, NetworkConnection> S_READY;
    public static Action<NetMessage, NetworkConnection> S_SWITCHTEAM;
    public static Action<NetMessage, NetworkConnection> S_START;

    // Net Tank messages
    public static Action<NetMessage> C_T_TRANSFORM;
    public static Action<NetMessage> C_T_INPUT;
    public static Action<NetMessage> C_T_TOWER_INPUT;
    public static Action<NetMessage> C_T_TOWER_ROTATION;
    public static Action<NetMessage, NetworkConnection> S_T_TRANSFORM;
    public static Action<NetMessage, NetworkConnection> S_T_INPUT;
    public static Action<NetMessage, NetworkConnection> S_T_TOWER_INPUT;
    public static Action<NetMessage, NetworkConnection> S_T_TOWER_ROTATION;


    public static void OnData(ref DataStreamReader streamReader, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var OpCode = (OpCode)streamReader.ReadByte();
        switch (OpCode)
        {
            case OpCode.PING:
                msg = new NetPing(ref streamReader);
                break;

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

            //Tank
            case OpCode.T_INPUT:
                msg = new NetTInput(ref streamReader);
                break;

            case OpCode.T_TRANSFORM:
                msg = new NetTTransform(ref streamReader);
                break;

            case OpCode.T_TOWER_INPUT:
                msg = new NetTTowerInput(ref streamReader);
                break;

            case OpCode.T_TOWER_ROTATION:
                msg = new NetTTowerRotation(ref streamReader);
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
