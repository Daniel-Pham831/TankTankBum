using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    This class is for handling tank movement
*/
public class TankMovement : MonoBehaviour
{
    private Rigidbody rb;
    private TankInformation localTankInfo;
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 90f;


    private float timeBetweenEachSend = 0.1f;
    private float nextSendTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        localTankInfo = GetComponent<TankInformation>();
    }

    private void Start()
    {
        registerToEvent(true);
        nextSendTime = Time.time + timeBetweenEachSend;

    }

    private void Update()
    {
        if (Time.time >= nextSendTime)
        {
            nextSendTime = Time.time + timeBetweenEachSend;
            Client.Singleton.SendToServer(new NetTTransform(localTankInfo.ID, rb.position, rb.rotation));
        }
    }


    void FixedUpdate()
    {
        if (localTankInfo.IsLocalPlayer)
            PlayerInput();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Should self-made a smooth input here

        if (verticalInput != 0)
        {
            if (verticalInput < 0)
            {
                horizontalInput *= -1;
            }
        }

        if (new Vector2(horizontalInput, verticalInput) != Vector2.zero)
            Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, horizontalInput, verticalInput));
    }


    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_TRANSFORM += OnClientReceivedTMoveMessage;
        }
        else
        {
            NetUtility.C_T_TRANSFORM -= OnClientReceivedTMoveMessage;
        }
    }

    private void OnClientReceivedTMoveMessage(NetMessage message)
    {
        NetTTransform tMoveMessage = message as NetTTransform;

        if (localTankInfo.ID != tMoveMessage.ID) return;

        Move(tMoveMessage.Position, tMoveMessage.Rotation);
    }

    private void Move(Vector3 position, Quaternion rotation)
    {
        rb.MovePosition(position);
        rb.MoveRotation(rotation);
    }

}
