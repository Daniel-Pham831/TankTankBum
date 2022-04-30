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
    [SerializeField] private GameObject tankNamePrefab;
    [SerializeField] private GameObject localTankCameraPrefab;
    [SerializeField] private Material blueTankMaterial;
    [SerializeField] private Material redTankMaterial;
    [SerializeField] private GameObject[] spawnPositions;

    // Tanks data
    public TankCamera LocalTankCamera { get; set; }
    public TankInformation LocalTankInformation { get; set; }
    public Dictionary<byte, TankInformation> TankInformations { get; set; }
    public Dictionary<byte, GameObject> TankObjects { get; set; }
    public Dictionary<byte, Rigidbody> TankRigidbodies { get; set; }
    public Dictionary<byte, TankName> TankNames { get; set; }


    private void Awake()
    {
        Singleton = this;
        TankInformations = new Dictionary<byte, TankInformation>();
        TankObjects = new Dictionary<byte, GameObject>();
        TankRigidbodies = new Dictionary<byte, Rigidbody>();
        TankNames = new Dictionary<byte, TankName>();

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

        SpawnTank(myPlayer.Id, myPlayer.Team, myPlayer.Name, true, clientInformation.IsHost);

        foreach (Player player in otherPlayers)
        {
            SpawnTank(player.Id, player.Team, player.Name, false, clientInformation.IsHost);
        }

        if (clientInformation.IsHost)
        {
            TankServerManager.Singleton.TankRigidbodies = TankRigidbodies;
        }

        SetupAllNamesData();
    }



    private void SpawnTank(byte id, Team team, string name, bool isLocalPlayer, bool isHost)
    {
        GameObject tank = Instantiate(tankPrefab, spawnPositions[id].transform.position, Quaternion.identity);
        Rigidbody tankRigid = tank.GetComponent<Rigidbody>();
        TankInformation tankInformation = tank.GetComponent<TankInformation>();
        tankInformation.ID = id;
        tankInformation.Team = team;
        tankInformation.IsLocalPlayer = isLocalPlayer;
        tankInformation.IsHost = isHost;

        if (isLocalPlayer)
        {
            LocalTankInformation = tankInformation;
            SetLocalTankCamera(tank, team);
        }
        else
        {
            SetTankName(id, tank, name); //Only show other tanks' name
        }
        SetTankColorBasedOnTeam(tank, team);

        TankInformations.Add(tankInformation.ID, tankInformation);
        TankObjects.Add(tankInformation.ID, tank);
        TankRigidbodies.Add(tankInformation.ID, tankRigid);
    }

    private void SetLocalTankCamera(GameObject tank, Team team)
    {
        GameObject localTankCameraObject = Instantiate(localTankCameraPrefab);
        TankCamera localTankCameraScript = localTankCameraObject.GetComponent<TankCamera>();

        localTankCameraScript.SetupTankCamera(tank, team == Team.Blue ? Role.Defender : Role.Attacker);

        LocalTankCamera = localTankCameraScript;
    }

    private void SetTankName(byte id, GameObject tank, string name)
    {
        GameObject tankNameObject = Instantiate(tankNamePrefab);
        TankName tankNameScript = tankNameObject.GetComponent<TankName>();
        tankNameScript.SetUpTankName(tank, name);

        TankNames.Add(id, tankNameScript);
    }

    private void SetTankColorBasedOnTeam(GameObject tank, Team team)
    {
        MeshRenderer[] meshRenderers = tank.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = team == Team.Blue ? blueTankMaterial : redTankMaterial;
        }
    }

    private void SetupAllNamesData()
    {
        foreach (byte id in TankNames.Keys)
        {
            TankNames[id].SetNameRot(LocalTankCamera.GetActualCameraRot);
            TankNames[id].SetNameColor(LocalTankInformation.Team, TankInformations[id].Team);
        }
    }
    #endregion
}