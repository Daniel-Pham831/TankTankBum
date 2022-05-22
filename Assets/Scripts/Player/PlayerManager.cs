using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Singleton { get; private set; }
    [HideInInspector] public Player MyPlayer;
    public Dictionary<byte, Player> Players;
    public bool IsLocalPlayer => MyPlayer != null ? MyPlayer.IsLocalPlayer : false;

    public Action<Player> PlayerManagerIsReady;
    public Action<TankCamera> LocalCameraIsready;

    private void Awake()
    {
        Singleton = this;
        Players = new Dictionary<byte, Player>();
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
            ClientInformation.Singleton.StartGame += OnStartGame;
        }
        else
        {
            ClientInformation.Singleton.StartGame -= OnStartGame;
        }
    }

    private void OnStartGame()
    {
        ClientInformation clientInformation = ClientInformation.Singleton;
        SlotPlayerInformation myPlayer = clientInformation.MyPlayerInformation;
        List<SlotPlayerInformation> otherPlayers = clientInformation.PlayerList;

        MyPlayer = new Player(myPlayer.Id, myPlayer.Team, myPlayer.Name, true, myPlayer.IsHost);

        AddPlayer(MyPlayer);
        foreach (var player in otherPlayers)
        {
            AddPlayer(new Player(player.Id, player.Team, player.Name, false, player.IsHost));
        }

        PlayerManagerIsReady?.Invoke(MyPlayer);
    }

    public void AddPlayer(Player newPlayer)
    {
        Players.Add(newPlayer.ID, newPlayer);
    }

    public void RemovePlayer(byte id)
    {
        Players.Remove(id);
    }

    public Player GetPlayer(byte id)
    {
        return Players[id];
    }
}
