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
    private List<byte> isInCountDown;

    private void Awake()
    {
        Singleton = this;
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
            NetUtility.S_T_KILL += OnServerReceivedTKillMessage;
        }
        else
        {
            NetUtility.S_T_SPAWN_REQ -= OnServerReceivedTankSpawnRequestMessage;
            NetUtility.S_T_SPAWN -= OnServerReceivedTankSpawnMessage;
            NetUtility.S_T_DIE -= OnServerReceivedTDieMessage;
            NetUtility.S_T_KILL -= OnServerReceivedTKillMessage;
        }
    }

    private void OnServerReceivedTKillMessage(NetMessage message, NetworkConnection sender)
    {
        Server.Singleton.BroadCast(message as NetTKill);
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

    private void OnServerReceivedTDieMessage(NetMessage message, NetworkConnection sender)
    {
        NetTDie tankDieMessage = message as NetTDie;
        Player player = PlayerManager.Singleton.GetPlayer(tankDieMessage.ID);
        float countDuration = tankSpawnerData.GetRespawnTime(player.Role);
        tankDieMessage.NextSpawnDuration = countDuration;

        // At this point a tank just die so send the dead message to all player
        // Which contains the death time, use this to show UI
        Server.Singleton.BroadCast(tankDieMessage);

        // after a countDuration send a spawn message to the death player
        HandleSpawnTank(tankDieMessage.ID, player, countDuration, sender);
    }

    private void HandleSpawnTank(byte id, Player player, float countDuration, NetworkConnection sender)
    {
        if (!isInCountDown.Contains(id))
        {
            StartCoroutine(SendSpawnMessageCountDown(id, player, countDuration, sender));
        }
    }

    /// <summary>
    /// This function will send a NetTSpawnReq back to client after a countDown which based on the RoleController
    /// </summary>
    private IEnumerator SendSpawnMessageCountDown(byte id, Player player, float countDuration, NetworkConnection sender)
    {
        isInCountDown.Add(id);

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
