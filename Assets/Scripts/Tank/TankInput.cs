using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is for sending input to server
*/
public class TankInput : MonoBehaviour
{
    private TankInformation localTankInfo;
    private InputSystem inputsystem;

    // for tank movement
    private Vector2 currentMovementInputVector;
    private Vector2 smoothInputVelocity;

    [SerializeField] private float smoothInputSpeed = .1f;

    private void Awake()
    {
        localTankInfo = GetComponent<TankInformation>();
        inputsystem = InputEventManager.Singleton.Inputsystem;
    }

    private void Start()
    {
        if (localTankInfo.IsLocalPlayer)
        {
            inputsystem.Tank.TowerRotation.performed += OnTowerRotationInputPerformed;
            inputsystem.Tank.Fire.performed += OnFireInputPerformed;
        }
    }

    private void OnTowerRotationInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Client.Singleton.SendToServer(new NetTTowerInput(localTankInfo.ID, context.ReadValue<float>()));
    }

    private void OnFireInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Client.Singleton.SendToServer(new NetTFireInput(localTankInfo.ID));
    }

    private void Update()
    {
        if (!localTankInfo.IsLocalPlayer) return;

        MovementInput();
    }

    private void MovementInput()
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

        currentMovementInputVector = Vector2.SmoothDamp(currentMovementInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);
        if (currentMovementInputVector != Vector2.zero)
        {
            Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, currentMovementInputVector.x, currentMovementInputVector.y)); //Has smoothing effect
            // Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, inputVector.x, inputVector.y)); //Input Raw without smoothing
        }
    }
}
