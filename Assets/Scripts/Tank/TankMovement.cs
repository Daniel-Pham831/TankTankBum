using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    This class is for handling tank movement
*/
public class TankMovement : MonoBehaviour
{
    private InputSystem inputsystem;
    private Rigidbody localRb;
    private TankInformation localTankInfo;

    private void Awake()
    {
        localRb = GetComponent<Rigidbody>();
        localTankInfo = GetComponent<TankInformation>();
        inputsystem = new InputSystem();
        inputsystem.Tank.Enable();
    }

    private void Start()
    {
        registerToEvent(true);
    }

    void FixedUpdate()
    {
        if (localTankInfo.IsLocalPlayer)
            TankInput();
    }

    private void TankInput()
    {
        Vector2 inputVector = inputsystem.Tank.Movement.ReadValue<Vector2>();
        float horizontalInput = inputVector.x;
        float verticalInput = inputVector.y;

        //need to smooth here

        if (verticalInput != 0)
        {
            if (verticalInput < 0)
            {
                horizontalInput *= -1;
            }
        }
        else
        {
            // If the player is NOT pressing down on the verticalInputs(W,S) then that means no rotation for the player
            horizontalInput = 0;
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
        NetTTransform tTransformMessage = message as NetTTransform;

        if (localTankInfo.ID != tTransformMessage.ID) return;

        Debug.Log("Client received nettransform");
        Move(tTransformMessage.Position, tTransformMessage.Rotation);
    }

    private void Move(Vector3 position, Quaternion rotation)
    {
        localRb.MovePosition(position);
        localRb.MoveRotation(rotation);
    }
}
