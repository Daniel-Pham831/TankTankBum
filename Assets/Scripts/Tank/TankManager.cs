using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public static TankManager Singleton { get; private set; }
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject[] spawnPositions;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        this.registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            ClientInformation.Singleton.StartGame += OnStartGame;

            NetUtility.S_TMOVE += OnServerReceivedTMoveMessage;
        }
        else
        {
            ClientInformation.Singleton.StartGame -= OnStartGame;

            NetUtility.S_TMOVE += OnServerReceivedTMoveMessage;
        }
    }

    private void OnServerReceivedTMoveMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        Server.Singleton.BroadCastExcept(message, sentPlayer);
    }

    private void OnStartGame()
    {
        ClientInformation clientInformation = ClientInformation.Singleton;
        Player myPlayer = clientInformation.MyPlayerInformation;
        List<Player> otherPlayers = clientInformation.PlayerList;

        this.SpawnTank(myPlayer.Id, myPlayer.Team, true, clientInformation.IsHost);

        foreach (Player player in otherPlayers)
        {
            this.SpawnTank(player.Id, player.Team, false, clientInformation.IsHost);
        }
    }

    private void SpawnTank(byte id, Team team, bool isOwner, bool isHost)
    {
        GameObject tank = Instantiate(this.tankPrefab, this.spawnPositions[id].transform.position, Quaternion.identity);
        TNetwork tNetwork = tank.GetComponent<TNetwork>();
        tNetwork.ID = id;
        tNetwork.Team = team;
        tNetwork.IsOwner = isOwner;
        tNetwork.IsHost = isHost;
    }
}
