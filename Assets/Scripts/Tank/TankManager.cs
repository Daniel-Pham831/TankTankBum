using System;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is for spawning and assign tank for each players in the game
*/
public class TankManager : MonoBehaviour
{
    public static TankManager Singleton { get; private set; }
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject tankNamePrefab;
    [SerializeField] private GameObject tankHealthPrefab;
    [SerializeField] private GameObject localTankCameraPrefab;
    [SerializeField] private Material blueTankMaterial;
    [SerializeField] private Material redTankMaterial;
    [SerializeField] private GameObject[] spawnPositions;
    [SerializeField] private float tankDefaultHealth = 100f;

    // Tanks data
    public TankCamera LocalTankCamera { get; set; }
    public TankInformation LocalTankInformation { get; set; }
    public Dictionary<byte, TankInformation> TankInformations { get; set; }
    public Dictionary<byte, Rigidbody> TankRigidbodies { get; set; }
    public Dictionary<byte, TankName> TankNames { get; set; }
    public Dictionary<byte, HealthBar> TankHealthBar { get; set; }


    private void Awake()
    {
        Singleton = this;
        TankInformations = new Dictionary<byte, TankInformation>();
        TankRigidbodies = new Dictionary<byte, Rigidbody>();
        TankNames = new Dictionary<byte, TankName>();
        TankHealthBar = new Dictionary<byte, HealthBar>();

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
        SetTankHealth(id, tank, tankDefaultHealth);
        SetTankColorBasedOnTeam(tank, team);

        TankInformations.Add(tankInformation.ID, tankInformation);
        TankRigidbodies.Add(tankInformation.ID, tankRigid);
        TankServerManager.Singleton.PreRbPosition.Add(id, tankRigid.position);
        TankServerManager.Singleton.PreRbRotation.Add(id, tankRigid.rotation);
    }

    private void SetTankHealth(byte id, GameObject tank, float tankDefaultHealth)
    {
        GameObject tankHealthObject = Instantiate(tankHealthPrefab);
        HealthBar heathBar = tankHealthObject.GetComponent<HealthBar>();
        heathBar.SetUpTankHealth(tank, tankDefaultHealth);
        heathBar.SetRot(LocalTankCamera.GetActualCameraRot);

        tank.GetComponent<TankHealth>().Health = tankDefaultHealth;
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