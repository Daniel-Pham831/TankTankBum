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
            TankManager.Singleton.OnTankSpawn += OnTankSpawn;
            TankManager.Singleton.OnTankDie += OnTankDie;
        }
        else
        {
            TankManager.Singleton.OnTankSpawn -= OnTankSpawn;
            TankManager.Singleton.OnTankDie -= OnTankDie;
        }
    }

    private void OnTankDie(byte id)
    {
        OnTankRemoved?.Invoke(id);

        GameObject deathTankObject = Tanks[id];
        Tanks.Remove(id);
        Destroy(deathTankObject);
    }

    private void OnTankSpawn(byte id, Vector3 spawnPostion)
    {
        Player player = PlayerManager.Singleton.GetPlayer(id);
        SpawnAndSetupTankData(player, spawnPostion);
    }

    /// <summary>
    /// Spawn tank, data, name, health bar, tank color
    /// </summary>
    private void SpawnAndSetupTankData(Player player, Vector3 spawnPosition)
    {
        // Spawn tank
        GameObject tank = Instantiate(tankSpawnerData.TankPrefab, spawnPosition, Quaternion.identity);
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

        SetTankHealth(tank, tankSpawnerData.TankDefaultHealth);
        SetTankColorBasedOnTeam(tank, player.Team);

        if (PlayerManager.Singleton.MyPlayer.IsHost)
            OnNewTankAdded?.Invoke(tank);
    }

    private void SetLocalTankCamera(GameObject tank, Role role)
    {
        GameObject localTankCameraObject =
        LocalTankCamera == null ? Instantiate(tankSpawnerData.LocalTankCameraPrefab) : LocalTankCamera.gameObject;

        TankCamera localTankCameraScript = localTankCameraObject.GetComponent<TankCamera>();

        localTankCameraScript.SetupTankCamera(tank, role);
        LocalTankCamera = localTankCameraScript;
    }

    private void SetTankName(GameObject tank, TankInformation tankInformation)
    {
        GameObject tankNameObject = Instantiate(tankSpawnerData.TankNamePrefab);
        TankName tankNameScript = tankNameObject.GetComponent<TankName>();
        tankNameScript.SetUpTankName(tank, tankInformation.Player.Name);
        tankNameScript.SetNameRot(LocalTankCamera.GetActualCameraRot);
        tankNameScript.SetNameColor(LocalTankInformation.Player.Team, tankInformation.Player.Team);
    }

    private void SetTankHealth(GameObject tank, float tankDefaultHealth)
    {
        GameObject tankHealthObject = Instantiate(tankSpawnerData.TankHealthPrefab);
        HealthBar heathBar = tankHealthObject.GetComponent<HealthBar>();
        heathBar.SetupHealthBar(tank, tankDefaultHealth, LocalTankCamera.GetActualCameraRot);

        tank.GetComponent<TankHealth>().Health = tankDefaultHealth;
    }

    private void SetTankColorBasedOnTeam(GameObject tank, Team team)
    {
        MeshRenderer[] meshRenderers = tank.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = team == Team.Blue ? tankSpawnerData.BlueTankMaterial : tankSpawnerData.RedTankMaterial;
        }
    }
}
