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

    public bool IsLocalPlayer => LocalTankInformation != null ? LocalTankInformation.IsLocalPlayer : false;

    public Action<GameObject> OnNewTankAdded;
    public Action<byte> OnTankRemoved;

    private void Awake()
    {
        Singleton = this;
        Tanks = new Dictionary<byte, GameObject>();

        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        registerToEvent(true);
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.P))
        {
            ClientInformation clientInformation = ClientInformation.Singleton;
            SlotPlayerInformation myPlayer = clientInformation.MyPlayerInformation;
            List<SlotPlayerInformation> otherPlayers = clientInformation.PlayerList;

            SpawnAndSetupTankData(myPlayer.Id, myPlayer.Team, myPlayer.Name, true, clientInformation.IsHost);
        }
    }


    #region Networking Functions

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            ClientInformation.Singleton.StartGame += OnStartGame;
            NetUtility.C_T_DIE += OnClientReceivedTDieMessage;
        }
        else
        {
            ClientInformation.Singleton.StartGame -= OnStartGame;
            NetUtility.C_T_DIE -= OnClientReceivedTDieMessage;
        }
    }

    private void OnClientReceivedTDieMessage(NetMessage message)
    {
        byte deathTankID = (message as NetTDie).ID;
        OnTankRemoved?.Invoke(deathTankID);

        GameObject deathTankObject = Tanks[deathTankID];
        Tanks.Remove(deathTankID);
        Destroy(deathTankObject);
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
    }

    private void SetLocalTankCamera(GameObject tank, Team team)
    {
        GameObject localTankCameraObject =
        LocalTankCamera == null ? Instantiate(localTankCameraPrefab) : LocalTankCamera.gameObject;

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