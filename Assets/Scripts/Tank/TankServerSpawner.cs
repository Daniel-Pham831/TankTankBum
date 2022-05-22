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
    public TankSpawnerData tankSpawnerData;
    private List<byte> hasBeenSpawned;
    private List<byte> isInCountDown;

    private void Awake()
    {
        Singleton = this;
        hasBeenSpawned = new List<byte>();
        isInCountDown = new List<byte>();
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
            NetUtility.S_T_DIE += OnServerReceivedTDieMessage;
        }
        else
        {
            NetUtility.S_T_SPAWN_REQ -= OnServerReceivedTankSpawnRequestMessage;
            NetUtility.S_T_SPAWN -= OnServerReceivedTankSpawnMessage;
            NetUtility.S_T_DIE -= OnServerReceivedTDieMessage;
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
        HandleSpawnTank(tSpawnReqMessage.ID, sender);

        // Player sendPlayerInfo = PlayerManager.Singleton.GetPlayer(tSpawnReqMessage.ID);

        // Vector3 newSpawnPosition = spawnPosition.GetPosition(sendPlayerInfo.Role).position;

        // Server.Singleton.SendToClient(sender, new NetTSpawnReq(tSpawnReqMessage.ID, newSpawnPosition));
    }

    private void OnServerReceivedTDieMessage(NetMessage message, NetworkConnection sender)
    {
        // At this point a tank just die
        Server.Singleton.BroadCast(message as NetTDie);
    }
    private void HandleSpawnTank(byte id, NetworkConnection sender)
    {
        // if this player(id): has never been spawn before, then allow to spawn with out waitng
        if (!hasBeenSpawned.Contains(id))
        {
            hasBeenSpawned.Add(id);
            StartCoroutine(SendSpawnMessageCountDown(id, sender, true));
        }
        else
        {
            if (!isInCountDown.Contains(id))
            {
                StartCoroutine(SendSpawnMessageCountDown(id, sender));
            }
        }
    }

    /// <summary>
    /// This function will send a NetTSpawnReq back to client after a countDown which based on the RoleController
    /// </summary>
    private IEnumerator SendSpawnMessageCountDown(byte id, NetworkConnection sender, bool shouldSendImmediately = false)
    {
        isInCountDown.Add(id);
        Player player = PlayerManager.Singleton.GetPlayer(id);
        float countDuration = tankSpawnerData.GetRespawnTime(player.Role);
        if (shouldSendImmediately)
            countDuration = 0;

        while (countDuration > 0)
        {
            countDuration -= Time.deltaTime;
            yield return null;
        }

        isInCountDown.Remove(id);

        Vector3 newSpawnPosition = spawnPosition.GetPosition(player.Role).position;

        Server.Singleton.SendToClient(sender, new NetTSpawnReq(id, newSpawnPosition));
    }
}
