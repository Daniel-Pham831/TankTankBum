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

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    [SerializeField] private float smoothInputSpeed = .1f;


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

        if (inputVector.y != 0)
        {
            if (inputVector.y < 0)
            {
                inputVector.x *= -1;
            }
        }
        else
        {
            // If the player is NOT pressing down on the verticalInputs(W,S) then that means no rotation for the player
            inputVector.x = 0;
        }

        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);

        if (currentInputVector != Vector2.zero)
            Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, currentInputVector.x, currentInputVector.y));
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
