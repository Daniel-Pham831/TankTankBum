using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Singleton { get; private set; }

    public Dictionary<byte, Player> Players;

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
            ClientInformation.Singleton.StartGame += OnStartGame;
        }
        else
        {
            ClientInformation.Singleton.StartGame -= OnStartGame;
        }
    }

    private void OnStartGame()
    {
        Players = new Dictionary<byte, Player>();

        ClientInformation clientInformation = ClientInformation.Singleton;
        SlotPlayerInformation myPlayer = clientInformation.MyPlayerInformation;
        List<SlotPlayerInformation> otherPlayers = clientInformation.PlayerList;

        AddPlayer(new Player(myPlayer.Id, myPlayer.Team, myPlayer.Name, true, myPlayer.IsHost));

        foreach (var player in otherPlayers)
        {
            AddPlayer(new Player(player.Id, player.Team, player.Name, false, player.IsHost));
        }
    }

    public void AddPlayer(Player newPlayer)
    {
        Players.Add(newPlayer.ID, newPlayer);
    }

    public void RemovePlayer(byte id)
    {
        Players.Remove(id);
    }
}
