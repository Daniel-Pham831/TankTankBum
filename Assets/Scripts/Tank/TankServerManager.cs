using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

/*
    This class in only available in the Server side.
    this class is for handling all tanks inputs and movement
*/
public class TankServerManager : MonoBehaviour
{
    #region UnityFunctions
    public static TankServerManager Singleton { get; private set; }

    // Tank Movement
    [SerializeField] private float tankMoveSpeed = 10f;
    [SerializeField] private float tankRotateSpeed = 2.8f;

    // Tank tower rotation
    [SerializeField] private float towerRotationAngle = 45f;

    // Tank Grenade default speed;
    [SerializeField] private float grenadeSpeed = 30f;
    private float timeBetweenEachTFire = 0.5f;

    private float timeBetweenEachSend = 0.05f;
    private float nextSendTime;

    public Dictionary<byte, Rigidbody> TankRigidbodies;
    public Dictionary<byte, Vector3> PreRbPosition;
    public Dictionary<byte, Quaternion> PreRbRotation;
    public Dictionary<byte, float> NextSendTFireTime;


    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        TankRigidbodies = new Dictionary<byte, Rigidbody>();
        PreRbPosition = new Dictionary<byte, Vector3>();
        PreRbRotation = new Dictionary<byte, Quaternion>();
        NextSendTFireTime = new Dictionary<byte, float>();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        registerToEvent(true);
    }

    private void Update()
    {
        if (IsSendable())
            SendTransformToAll();
    }

    private bool IsSendable()
    {
        if (Time.time >= nextSendTime)
        {
            nextSendTime = Time.time + timeBetweenEachSend;
            return true;
        }

        return false;
    }

    private void SendTransformToAll()
    {
        foreach (byte id in TankRigidbodies.Keys)
        {
            if (PreRbPosition[id] != TankRigidbodies[id].position)
            {
                PreRbPosition[id] = TankRigidbodies[id].position;
                Server.Singleton.BroadCast(new NetTPosition(id, TankRigidbodies[id].position));
            }

            if (PreRbRotation[id].eulerAngles != TankRigidbodies[id].rotation.eulerAngles)
            {
                PreRbRotation[id] = TankRigidbodies[id].rotation;
                Server.Singleton.BroadCast(new NetTRotation(id, TankRigidbodies[id].transform.forward));
            }
        }
    }

    #endregion

    #region Server Events Handling Functions
    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_T_INPUT += OnServerReceivedTInputMessage;
            NetUtility.S_T_TOWER_INPUT += OnServerReceivedTTowerInputMessage;
            NetUtility.S_T_FIRE_INPUT += OnServerReceivedTFireInputMessage;

            TankSpawner.Singleton.OnNewTankAdded += OnNewTankAdded;
            TankSpawner.Singleton.OnTankRemoved += OnTankRemoved;
        }
        else
        {
            NetUtility.S_T_INPUT -= OnServerReceivedTInputMessage;
            NetUtility.S_T_TOWER_INPUT -= OnServerReceivedTTowerInputMessage;
            NetUtility.S_T_FIRE_INPUT -= OnServerReceivedTFireInputMessage;

            TankSpawner.Singleton.OnNewTankAdded -= OnNewTankAdded;
            TankSpawner.Singleton.OnTankRemoved -= OnTankRemoved;
        }
    }

    private void OnTankRemoved(byte removedTankID)
    {
        TankRigidbodies.Remove(removedTankID);
        PreRbPosition.Remove(removedTankID);
        PreRbRotation.Remove(removedTankID);
        NextSendTFireTime.Remove(removedTankID);
    }

    private void OnNewTankAdded(GameObject addedTank)
    {
        TankInformation tankInformation = addedTank.GetComponent<TankInformation>();

        TankRigidbodies.Add(tankInformation.Player.ID, addedTank.GetComponent<Rigidbody>());
        PreRbPosition.Add(tankInformation.Player.ID, addedTank.transform.position);
        PreRbRotation.Add(tankInformation.Player.ID, addedTank.transform.rotation);
        NextSendTFireTime.Add(tankInformation.Player.ID, 0);
    }

    private void OnServerReceivedTFireInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTFireInput tFireInputMessage = message as NetTFireInput;
        if (!TankRigidbodies.ContainsKey(tFireInputMessage.ID)) return;


        float nextSendTFireTime;
        if (!NextSendTFireTime.TryGetValue(tFireInputMessage.ID, out nextSendTFireTime))
            NextSendTFireTime[tFireInputMessage.ID] = 0;

        nextSendTFireTime = NextSendTFireTime[tFireInputMessage.ID];
        if (Time.time >= nextSendTFireTime)
        {
            NextSendTFireTime[tFireInputMessage.ID] = Time.time + timeBetweenEachTFire;

            GameObject sentPlayerTankTower = TankRigidbodies[tFireInputMessage.ID].GetComponent<TankMovement>().TankTower;
            tFireInputMessage.FireDirection = sentPlayerTankTower.transform.forward;
            tFireInputMessage.Speed = grenadeSpeed;
            Server.Singleton.BroadCast(tFireInputMessage);
        }
    }

    private void OnServerReceivedTTowerInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTTowerInput tankTowerInputMessage = message as NetTTowerInput;
        if (!TankRigidbodies.ContainsKey(tankTowerInputMessage.ID)) return;

        GameObject sentPlayerTankTower = TankRigidbodies[tankTowerInputMessage.ID].GetComponent<TankMovement>().TankTower;

        sentPlayerTankTower.transform.localEulerAngles += tankTowerInputMessage.RotationInput * towerRotationAngle * Vector3.up;
        Server.Singleton.BroadCast(new NetTTowerRotation(tankTowerInputMessage.ID, sentPlayerTankTower.transform.localEulerAngles));
    }

    private void OnServerReceivedTInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTInput tankInputMessage = message as NetTInput;
        if (!TankRigidbodies.ContainsKey(tankInputMessage.ID)) return;

        Server.Singleton.BroadCast(new NetTInput(tankInputMessage.ID, tankInputMessage.HorizontalInput, tankInputMessage.VerticalInput));

        if (!Mathf.Approximately(tankInputMessage.VerticalInput, 0f))
        {
            ConvertAndBroadCast(tankInputMessage);
        }
    }

    /*
        At this point, Server just received an movement input message from sentPlayer
        Server needs to calculate the sentPlayer position,rotation and send it back to all players
    */
    private void ConvertAndBroadCast(NetTInput tankInputMessage)
    {
        if (tankInputMessage.VerticalInput < 0)
        {
            tankInputMessage.HorizontalInput *= -1;
        }

        Rigidbody sentPlayerRigidbody = TankRigidbodies[tankInputMessage.ID];

        MoveSentPlayerRigidBodyBasedOnInput(ref sentPlayerRigidbody, tankInputMessage.HorizontalInput, tankInputMessage.VerticalInput);
        Server.Singleton.BroadCast(new NetTRotation(tankInputMessage.ID, sentPlayerRigidbody.transform.forward));
        Server.Singleton.BroadCast(new NetTVelocity(tankInputMessage.ID, sentPlayerRigidbody.velocity));
    }

    #endregion

    #region Calculating Functions
    private void MoveSentPlayerRigidBodyBasedOnInput(ref Rigidbody rb, float horizontalInput, float verticalInput)
    {
        rb.velocity = (rb.transform.forward * verticalInput * tankMoveSpeed) + Vector3.up * rb.velocity.y;
        rb.transform.localEulerAngles += horizontalInput * tankRotateSpeed * Vector3.up;
    }
    #endregion
}
