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

    private void OnDestroy()
    {
        registerToEvent(false);
    }

    #endregion

    #region Server Events Handling Functions
    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_T_INPUT += OnServerReceivedTInputMessage;
            NetUtility.S_T_TRANSFORM += OnServerReceivedTMoveMessage;
        }
        else
        {
            NetUtility.S_T_INPUT -= OnServerReceivedTInputMessage;
            NetUtility.S_T_TRANSFORM -= OnServerReceivedTMoveMessage;
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

        CalculateTankRigidBodyBasedOnInput(ref sentPlayerRigidbody, tankInputMessage.HorizontalInput, tankInputMessage.VerticalInput);

        Server.Singleton.BroadCast(new NetTTransform(tankInputMessage.ID, sentPlayerRigidbody.position, sentPlayerRigidbody.rotation));
    }

    private void OnServerReceivedTMoveMessage(NetMessage message, NetworkConnection sentPlayer)
    {
        Server.Singleton.BroadCastExcept(message, sentPlayer);
    }


    #endregion

    #region Calculating Functions
    private void CalculateTankRigidBodyBasedOnInput(ref Rigidbody rb, float horizontalInput, float verticalInput)
    {
        rb.MovePosition(rb.transform.position + rb.transform.forward * verticalInput * tankMoveSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.transform.rotation * Quaternion.Euler(Vector3.up * horizontalInput * tankRotateSpeed * Time.fixedDeltaTime));
    }

    #endregion
}
