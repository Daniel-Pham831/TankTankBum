using System;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is for storing all tanks which are alive data
    Every client will have this
*/
public class TankManager : MonoBehaviour
{
    public static TankManager Singleton { get; private set; }

    // Tanks data
    [HideInInspector] public TankInformation LocalTankInformation;
    public Action<byte, Vector3> OnTankSpawn;
    public Action<byte> OnTankDie;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        registerToEvent(true);
    }

    #region Events, messages
    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            PlayerManager.Singleton.PlayerManagerIsReady += PlayerManagerIsReady;

            NetUtility.C_T_SPAWN_REQ += OnClientReceivedTSpawnRequestMessage;
            NetUtility.C_T_SPAWN += OnClientReceivedTSpawnMessage;
            NetUtility.C_T_DIE += OnClientReceivedTDieMessage;
        }
        else
        {
            PlayerManager.Singleton.PlayerManagerIsReady -= PlayerManagerIsReady;

            NetUtility.C_T_SPAWN_REQ -= OnClientReceivedTSpawnRequestMessage;
            NetUtility.C_T_SPAWN -= OnClientReceivedTSpawnMessage;
            NetUtility.C_T_DIE -= OnClientReceivedTDieMessage;
        }
    }

    private void PlayerManagerIsReady(Player player)
    {
        gameObject.AddComponent<TankInformation>();
        LocalTankInformation = GetComponent<TankInformation>();
        LocalTankInformation.Player = player;

        Client.Singleton.SendToServer(new NetTSpawnReq(player.ID));
    }

    // This is for local player, because other tank data dependent on local tank data
    private void OnClientReceivedTSpawnRequestMessage(NetMessage message)
    {
        NetTSpawnReq tSpawnReqMessage = message as NetTSpawnReq;

        OnTankSpawn?.Invoke(tSpawnReqMessage.ID, tSpawnReqMessage.Position);
        Client.Singleton.SendToServer(new NetTSpawn(tSpawnReqMessage.ID, tSpawnReqMessage.Position));
    }

    // This is for all player
    private void OnClientReceivedTSpawnMessage(NetMessage message)
    {
        NetTSpawn tSpawnMessage = message as NetTSpawn;

        OnTankSpawn?.Invoke(tSpawnMessage.ID, tSpawnMessage.Position);
    }

    private void OnClientReceivedTDieMessage(NetMessage message)
    {
        OnTankDie?.Invoke((message as NetTDie).ID);
    }

    #endregion
}
