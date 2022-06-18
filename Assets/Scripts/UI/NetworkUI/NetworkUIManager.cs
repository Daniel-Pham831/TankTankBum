using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif


public class NetworkUIManager : MonoBehaviour
{
    public static NetworkUIManager Singleton { get; private set; }
    public TMP_Text testText;
    public GameObject LocalPlayerDeathUI;

    public Action<byte, float> OnLocalPlayerDeadUI;
    public Action<Player, Player> OnPlayerKilledPlayer;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        registerToEvent(true);
    }
    private void OnDestroy()
    {
        registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_DIE += OnClientReceivedTDieMessage;
            NetUtility.C_T_KILL += OnClientReceivedTKillMessage;
        }
        else
        {
            NetUtility.C_T_DIE -= OnClientReceivedTDieMessage;
            NetUtility.C_T_KILL -= OnClientReceivedTKillMessage;
        }
    }

    private void OnClientReceivedTKillMessage(NetMessage message)
    {
        NetTKill tankKillMessage = message as NetTKill;
        Player killer = PlayerManager.Singleton.GetPlayer(tankKillMessage.KillerPlayerID);
        Player killed = PlayerManager.Singleton.GetPlayer(tankKillMessage.DeadPlayerID);

        OnPlayerKilledPlayer?.Invoke(killer, killed);
    }

    private void OnClientReceivedTDieMessage(NetMessage message)
    {
        NetTDie tankDieMessage = message as NetTDie;
        OnLocalPlayerDeadUI?.Invoke(tankDieMessage.ID, tankDieMessage.NextSpawnDuration);
    }
}
