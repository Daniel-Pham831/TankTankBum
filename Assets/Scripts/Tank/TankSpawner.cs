using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is for spawning and removing tanks
    Every client will have this
*/
public class TankSpawner : MonoBehaviour
{
    public static TankSpawner Singleton { get; private set; }
    [SerializeField] private TankSpawnerData tankSpawnerData;

    // Tanks data
    public TankInformation LocalTankInformation { get; set; }
    public TankCamera LocalTankCamera { get; set; }
    private bool IsLocalPlayer => LocalTankInformation != null ? LocalTankInformation.Player.IsLocalPlayer : false;
    public Dictionary<byte, GameObject> Tanks { get; set; }
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

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_SPAWN_REQ += OnClientReceivedTSpawnRequestMessage;
            NetUtility.C_T_SPAWN += OnClientReceivedTSpawnMessage;
            NetUtility.C_T_DIE += OnClientReceivedTDieMessage;
        }
        else
        {
            NetUtility.C_T_SPAWN_REQ -= OnClientReceivedTSpawnRequestMessage;
            NetUtility.C_T_SPAWN -= OnClientReceivedTSpawnMessage;
            NetUtility.C_T_DIE -= OnClientReceivedTDieMessage;
        }
    }

    private void OnClientReceivedTSpawnMessage(NetMessage message)
    {
        NetTSpawn tSpawnMessage = message as NetTSpawn;

        Player sendPlayerInfo = PlayerManager.Singleton.GetPlayer(tSpawnMessage.ID);

        SpawnAndSetupTankData(sendPlayerInfo, tSpawnMessage.Position);
    }

    private void OnClientReceivedTSpawnRequestMessage(NetMessage message)
    {
        NetTSpawnReq tSpawnReqMessage = message as NetTSpawnReq;

        Player sendPlayerInfo = PlayerManager.Singleton.GetPlayer(tSpawnReqMessage.ID);

        SpawnAndSetupTankData(sendPlayerInfo, tSpawnReqMessage.Position);
        Client.Singleton.SendToServer(new NetTSpawn(sendPlayerInfo.ID, tSpawnReqMessage.Position));
    }

    private void OnClientReceivedTDieMessage(NetMessage message)
    {
        byte deathTankID = (message as NetTDie).ID;
        OnTankRemoved?.Invoke(deathTankID);

        GameObject deathTankObject = Tanks[deathTankID];
        Tanks.Remove(deathTankID);
        Destroy(deathTankObject);
    }

    /// <summary>
    /// Spawn tank, data, name, health bar, tank color
    /// </summary>
    private void SpawnAndSetupTankData(Player player, Vector3 spawnPosition)
    {
        // Spawn tank
        GameObject tank = Instantiate(tankSpawnerData.tankPrefab, spawnPosition, Quaternion.identity);
        Rigidbody tankRigid = tank.GetComponent<Rigidbody>();

        // Setup tank data
        TankInformation tankInformation = tank.GetComponent<TankInformation>();
        tankInformation.Player = player;
        Tanks.Add(player.ID, tank);

        if (player.IsLocalPlayer)
        {
            LocalTankInformation = tankInformation;
            SetLocalTankCamera(tank, player.Role); // Sai o day , nen tao ra 1 scriptable object chua' camera position
        }
        else
        {
            SetTankName(tank, tankInformation); //Only show other tanks' name
        }

        SetTankHealth(tank, tankSpawnerData.tankDefaultHealth);
        SetTankColorBasedOnTeam(tank, player.Team);

        if (PlayerManager.Singleton.MyPlayer.IsHost)
            OnNewTankAdded?.Invoke(tank);
    }

    private void SetLocalTankCamera(GameObject tank, Role role)
    {
        GameObject localTankCameraObject =
        LocalTankCamera == null ? Instantiate(tankSpawnerData.localTankCameraPrefab) : LocalTankCamera.gameObject;

        TankCamera localTankCameraScript = localTankCameraObject.GetComponent<TankCamera>();

        localTankCameraScript.SetupTankCamera(tank, role);
        LocalTankCamera = localTankCameraScript;
    }

    private void SetTankName(GameObject tank, TankInformation tankInformation)
    {
        GameObject tankNameObject = Instantiate(tankSpawnerData.tankNamePrefab);
        TankName tankNameScript = tankNameObject.GetComponent<TankName>();
        tankNameScript.SetUpTankName(tank, tankInformation.Player.Name);
        tankNameScript.SetNameRot(LocalTankCamera.GetActualCameraRot);
        tankNameScript.SetNameColor(LocalTankInformation.Player.Team, tankInformation.Player.Team);
    }

    private void SetTankHealth(GameObject tank, float tankDefaultHealth)
    {
        GameObject tankHealthObject = Instantiate(tankSpawnerData.tankHealthPrefab);
        HealthBar heathBar = tankHealthObject.GetComponent<HealthBar>();
        heathBar.SetupHealthBar(tank, tankDefaultHealth, LocalTankCamera.GetActualCameraRot);

        tank.GetComponent<TankHealth>().Health = tankDefaultHealth;
    }

    private void SetTankColorBasedOnTeam(GameObject tank, Team team)
    {
        MeshRenderer[] meshRenderers = tank.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = team == Team.Blue ? tankSpawnerData.blueTankMaterial : tankSpawnerData.redTankMaterial;
        }
    }
}
