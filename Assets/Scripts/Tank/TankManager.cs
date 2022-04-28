using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;


/*
    This class is for spawning and assign tank for each players in the game
*/
public class TankManager : MonoBehaviour
{
    public static TankManager Singleton { get; private set; }
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject[] spawnPositions;

    [HideInInspector]
    public Dictionary<byte, GameObject> TankObjects { get; set; }
    public Dictionary<byte, Rigidbody> TankRigidbodies { get; set; }


    private void Awake()
    {
        Singleton = this;
        TankObjects = new Dictionary<byte, GameObject>();
        TankRigidbodies = new Dictionary<byte, Rigidbody>();
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        registerToEvent(true);
    }


    #region Networking Functions

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
        Player myPlayer = clientInformation.MyPlayerInformation;
        List<Player> otherPlayers = clientInformation.PlayerList;

        SpawnTank(myPlayer.Id, myPlayer.Team, true, clientInformation.IsHost);

        foreach (Player player in otherPlayers)
        {
            SpawnTank(player.Id, player.Team, false, clientInformation.IsHost);
        }

        if (clientInformation.IsHost)
        {
            TankServerManager.Singleton.TankRigidbodies = TankRigidbodies;
        }
    }

    private void SpawnTank(byte id, Team team, bool isOwner, bool isHost)
    {
        GameObject tank = Instantiate(tankPrefab, spawnPositions[id].transform.position, Quaternion.identity);
        Rigidbody tankRigid = tank.GetComponent<Rigidbody>();
        TankInformation tNetwork = tank.GetComponent<TankInformation>();
        tNetwork.ID = id;
        tNetwork.Team = team;
        tNetwork.IsLocalPlayer = isOwner;
        tNetwork.IsHost = isHost;
        Debug.Log($"Tank spawned with ID:{tNetwork.ID}");


        TankObjects.Add(tNetwork.ID, tank);
        TankRigidbodies.Add(tNetwork.ID, tankRigid);
    }
    #endregion
}