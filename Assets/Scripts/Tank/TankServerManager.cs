using System;
using System.Collections;
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

    [SerializeField] private float tankMoveSpeed;
    [SerializeField] private float tankRotateSpeed;
    public Dictionary<byte, Rigidbody> TankRigidbodies;

    private float timeBetweenEachSend = 0.1f;
    private float nextSendTime;


    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        TankRigidbodies = new Dictionary<byte, Rigidbody>();
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
            Server.Singleton.BroadCast(new NetTTransform(id, TankRigidbodies[id].position, TankRigidbodies[id].rotation));
        }
    }

    #endregion

    #region Server Events Handling Functions
    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_T_INPUT += OnServerReceivedTInputMessage;
        }
        else
        {
            NetUtility.S_T_INPUT -= OnServerReceivedTInputMessage;
        }
    }

    private void OnServerReceivedTInputMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        NetTInput tankInputMessage = message as NetTInput;
        /*
            At this point, Server just received an input message from sentPlayer
            Server needs to calculate the sentPlayer position,rotation and send it back to all players
        */

        Rigidbody sentPlayerRigidbody = TankRigidbodies[tankInputMessage.ID].GetComponent<Rigidbody>();

        MoveSentPlayerRigidBodyBasedOnInput(ref sentPlayerRigidbody, tankInputMessage.HorizontalInput, tankInputMessage.VerticalInput);

        Server.Singleton.BroadCast(new NetTTransform(tankInputMessage.ID, sentPlayerRigidbody.position, sentPlayerRigidbody.rotation));
    }

    #endregion

    #region Calculating Functions
    private void MoveSentPlayerRigidBodyBasedOnInput(ref Rigidbody rb, float horizontalInput, float verticalInput)
    {
        // should refactor this MovePosition and MoveRotation into rb.AddForce
        rb.MovePosition(rb.transform.position + rb.transform.forward * verticalInput * tankMoveSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.transform.rotation * Quaternion.Euler(Vector3.up * horizontalInput * tankRotateSpeed * Time.fixedDeltaTime));
    }

    #endregion
}
