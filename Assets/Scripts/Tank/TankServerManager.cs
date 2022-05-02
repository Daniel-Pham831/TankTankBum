using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

/*
    This class in only available in the Server side.
*/
public class TankServerManager : MonoBehaviour
{
    #region UnityFunctions
    public static TankServerManager Singleton { get; private set; }

    // Tank Movement
    [SerializeField] private float tankMoveSpeed = 10f;
    [SerializeField] private float tankRotateSpeed = 135f;

    // Tank tower rotation
    [SerializeField] private float towerRotationAngle = 45f;

    private float timeBetweenEachSend = 0.1f;
    private float nextSendTime;

    public Dictionary<byte, Rigidbody> TankRigidbodies;
    public Dictionary<byte, Vector3> PreRbPosition;
    public Dictionary<byte, Quaternion> PreRbRotation;


    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        TankRigidbodies = new Dictionary<byte, Rigidbody>();
        PreRbPosition = new Dictionary<byte, Vector3>();
        PreRbRotation = new Dictionary<byte, Quaternion>();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        registerToEvent(true);
    }

    private void LateUpdate()
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
            // Server.Singleton.BroadCast(new NetTTransform(id, TankRigidbodies[id].position, TankRigidbodies[id].rotation));
            if (PreRbPosition[id] != TankRigidbodies[id].position)
            {
                PreRbPosition[id] = TankRigidbodies[id].position;
                Server.Singleton.BroadCast(new NetTPosition(id, TankRigidbodies[id].position));
            }

            if (PreRbRotation[id].eulerAngles != TankRigidbodies[id].rotation.eulerAngles)
            {
                PreRbRotation[id] = TankRigidbodies[id].rotation;
                Server.Singleton.BroadCast(new NetTRotation(id, TankRigidbodies[id].rotation));
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
        }
        else
        {
            NetUtility.S_T_INPUT -= OnServerReceivedTInputMessage;
            NetUtility.S_T_TOWER_INPUT -= OnServerReceivedTTowerInputMessage;
            NetUtility.S_T_FIRE_INPUT -= OnServerReceivedTFireInputMessage;
        }
    }

    private void OnServerReceivedTFireInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        Server.Singleton.BroadCast(new NetTFireInput((message as NetTFireInput).ID));
    }

    private void OnServerReceivedTTowerInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTTowerInput tankTowerInputMessage = message as NetTTowerInput;

        GameObject sentPlayerTankTower = TankRigidbodies[tankTowerInputMessage.ID].GetComponent<TankMovement>().TankTower;

        sentPlayerTankTower.transform.localEulerAngles += tankTowerInputMessage.RotationInput * towerRotationAngle * Vector3.up;
        Server.Singleton.BroadCast(new NetTTowerRotation(tankTowerInputMessage.ID, sentPlayerTankTower.transform.localEulerAngles));
    }

    private void OnServerReceivedTInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTInput tankInputMessage = message as NetTInput;

        Server.Singleton.BroadCast(new NetTInput(tankInputMessage.ID, tankInputMessage.HorizontalInput, tankInputMessage.VerticalInput));

        if (!Mathf.Approximately(tankInputMessage.VerticalInput, 0f))
        {
            ConvertAndBroadCast(tankInputMessage);
        }
    }

    /*
        At this point, Server just received an input message from sentPlayer
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
        Server.Singleton.BroadCast(new NetTRotation(tankInputMessage.ID, sentPlayerRigidbody.rotation));
        Server.Singleton.BroadCast(new NetTVelocity(tankInputMessage.ID, sentPlayerRigidbody.velocity));
    }

    #endregion

    #region Calculating Functions
    private void MoveSentPlayerRigidBodyBasedOnInput(ref Rigidbody rb, float horizontalInput, float verticalInput)
    {
        rb.velocity = (rb.transform.forward * verticalInput * tankMoveSpeed) + Vector3.up * rb.velocity.y;
        rb.MoveRotation(rb.transform.rotation * Quaternion.Euler(Vector3.up * horizontalInput * tankRotateSpeed * Time.fixedDeltaTime));
    }
    #endregion
}
