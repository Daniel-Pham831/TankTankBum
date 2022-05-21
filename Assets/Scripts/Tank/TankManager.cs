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
            PlayerManager.Singleton.PlayerManagerIsReady += PlayerManagerIsReady;
        }
        else
        {
            PlayerManager.Singleton.PlayerManagerIsReady -= PlayerManagerIsReady;
        }
    }

    private void PlayerManagerIsReady(Player player)
    {
        gameObject.AddComponent<TankInformation>();
        LocalTankInformation = GetComponent<TankInformation>();
        LocalTankInformation.Player = player;
        Client.Singleton.SendToServer(new NetTSpawnReq(player.ID));
    }
}
