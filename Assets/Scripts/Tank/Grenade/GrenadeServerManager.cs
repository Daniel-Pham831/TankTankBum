using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

/*
    When a client sends an exploded grenade information to server
    This "server" class sending the exploded grenade information to other clients

*/
public class GrenadeServerManager : MonoBehaviour
{
    public static GrenadeServerManager Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_GRENADE_EXPLOSION += OnServerReceivedGrenadeExplosionMessage;
        }
        else
        {
            NetUtility.S_GRENADE_EXPLOSION -= OnServerReceivedGrenadeExplosionMessage;
        }
    }

    private void OnServerReceivedGrenadeExplosionMessage(NetMessage message, NetworkConnection sender)
    {
        Server.Singleton.BroadCast(message as NetGrenadeExplosion);
    }
}
