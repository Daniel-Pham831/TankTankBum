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
            NetUtility.S_T_SPAWN += OnServerReceivedTankSpawnMessage;
        }
        else
        {
            NetUtility.S_T_SPAWN -= OnServerReceivedTankSpawnMessage;
        }
    }

    private void OnServerReceivedTankSpawnMessage(NetMessage message, NetworkConnection sender)
    {
        NetTSpawn tSpawnMessage = message as NetTSpawn;
        Player sendPlayerInfo = PlayerManager.Singleton.GetPlayer(tSpawnMessage.ID);

        Vector3 newSpawnPosition = spawnPosition.GetPosition(sendPlayerInfo.Role).position;

        Server.Singleton.BroadCast(new NetTSpawn(tSpawnMessage.ID, newSpawnPosition));
    }
}
