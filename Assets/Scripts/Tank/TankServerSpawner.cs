using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

/*
    Only host can have this 
*/
public class TankServerSpawner : MonoBehaviour
{
    public static TankServerSpawner Singleton { get; private set; }
    public SpawnPosition spawnPosition;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_T_SPAWN_REQ += OnServerReceivedTankSpawnRequestMessage;
            NetUtility.S_T_SPAWN += OnServerReceivedTankSpawnMessage;
        }
        else
        {
            NetUtility.S_T_SPAWN_REQ -= OnServerReceivedTankSpawnRequestMessage;
            NetUtility.S_T_SPAWN -= OnServerReceivedTankSpawnMessage;
        }
    }

    private void OnServerReceivedTankSpawnMessage(NetMessage message, NetworkConnection sender)
    {
        NetTSpawn tSpawnMessage = message as NetTSpawn;
        Server.Singleton.BroadCastExcept(tSpawnMessage, sender);
    }

    private void OnServerReceivedTankSpawnRequestMessage(NetMessage message, NetworkConnection sender)
    {
        NetTSpawnReq tSpawnReqMessage = message as NetTSpawnReq;
        Player sendPlayerInfo = PlayerManager.Singleton.GetPlayer(tSpawnReqMessage.ID);

        Vector3 newSpawnPosition = spawnPosition.GetPosition(sendPlayerInfo.Role).position;

        Server.Singleton.SendToClient(sender, new NetTSpawnReq(tSpawnReqMessage.ID, newSpawnPosition));
    }
}
