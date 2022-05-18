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
    public Dictionary<byte, GameObject> Tanks { get; set; }
    public Dictionary<byte, GameObject> HealthBars { get; set; }
    public Dictionary<byte, GameObject> Names { get; set; }

    public bool IsLocalPlayer => LocalTankInformation.IsLocalPlayer;

    public Action<GameObject> OnNewTankAdded;
    public Action<byte> OnTankRemoved;


    private void Awake()
    {
        Singleton = this;
        Tanks = new Dictionary<byte, GameObject>();
        HealthBars = new Dictionary<byte, GameObject>();
        Names = new Dictionary<byte, GameObject>();

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
        SlotPlayerInformation myPlayer = clientInformation.MyPlayerInformation;
        List<SlotPlayerInformation> otherPlayers = clientInformation.PlayerList;

        SpawnAndSetupTankData(myPlayer.Id, myPlayer.Team, myPlayer.Name, true, clientInformation.IsHost);

        foreach (SlotPlayerInformation player in otherPlayers)
        {
            SpawnAndSetupTankData(player.Id, player.Team, player.Name, false, clientInformation.IsHost);
        }
    }

    /// <summary>
    /// Spawn tank, data, name, health bar, tank color
    /// </summary>
    private void SpawnAndSetupTankData(byte id, Team team, string name, bool isLocalPlayer, bool isHost)
    {
        // Spawn tank
        GameObject tank = Instantiate(tankPrefab, spawnPositions[id].transform.position, Quaternion.identity);
        Rigidbody tankRigid = tank.GetComponent<Rigidbody>();

        // Setup tank data
        TankInformation tankInformation = tank.GetComponent<TankInformation>();
        tankInformation.Setup(id, team, isLocalPlayer, isHost);
        Tanks.Add(id, tank);

        if (isLocalPlayer)
        {
            LocalTankInformation = tankInformation;
            SetLocalTankCamera(tank, team);
        }
        else
        {
            SetTankName(tank, tankInformation, name); //Only show other tanks' name
        }

        SetTankHealth(id, tank, tankDefaultHealth);
        SetTankColorBasedOnTeam(tank, team);

        if (isHost)
            OnNewTankAdded?.Invoke(tank);
    }

    private void SetTankHealth(byte id, GameObject tank, float tankDefaultHealth)
    {
        GameObject tankHealthObject = Instantiate(tankHealthPrefab);
        HealthBar heathBar = tankHealthObject.GetComponent<HealthBar>();
        heathBar.SetUpTankHealth(tank, tankDefaultHealth);
        heathBar.SetRot(LocalTankCamera.GetActualCameraRot);

        tank.GetComponent<TankHealth>().Health = tankDefaultHealth;

        HealthBars.Add(id, tankHealthObject);
    }

    private void SetLocalTankCamera(GameObject tank, Team team)
    {
        GameObject localTankCameraObject = Instantiate(localTankCameraPrefab);
        TankCamera localTankCameraScript = localTankCameraObject.GetComponent<TankCamera>();

        localTankCameraScript.SetupTankCamera(tank, team == Team.Blue ? Role.Defender : Role.Attacker);
        LocalTankCamera = localTankCameraScript;
    }

    private void SetTankName(GameObject tank, TankInformation tankInformation, string name)
    {
        GameObject tankNameObject = Instantiate(tankNamePrefab);
        TankName tankNameScript = tankNameObject.GetComponent<TankName>();
        tankNameScript.SetUpTankName(tank, name);
        tankNameScript.SetNameRot(LocalTankCamera.GetActualCameraRot);
        tankNameScript.SetNameColor(LocalTankInformation.Team, tankInformation.Team);

        Names.Add(tankInformation.ID, tankNameObject);
    }

    private void SetTankColorBasedOnTeam(GameObject tank, Team team)
    {
        MeshRenderer[] meshRenderers = tank.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = team == Team.Blue ? blueTankMaterial : redTankMaterial;
        }
    }

    #endregion
}