using System;
using UnityEngine;

/*
    This class is for sending input to server
*/
public class TankInput : MonoBehaviour
{
    private TankInformation localTankInfo;
    private InputSystem inputsystem;

    /// <summary>
    /// For tank movement.
    /// </summary>
    private Vector2 currentMovementInputVector;
    private Vector2 smoothInputVelocity;
    private float towerRotation;
    private bool isRotating;

    // For tank fire
    private bool isFiring;

    [SerializeField]
    private float smoothInputSpeed = .1f;

    private void Awake()
    {
        localTankInfo = GetComponent<TankInformation>();
        inputsystem = InputEventManager.Singleton.Inputsystem;
    }

    private void Start()
    {
        if (localTankInfo.Player.IsLocalPlayer)
        {
            registerToEvent(true);
        }
    }

    private void OnDestroy()
    {
        if (localTankInfo.Player.IsLocalPlayer)
        {
            registerToEvent(false);
        }
    }
    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            inputsystem.Tank.TowerRotation.performed += OnTowerRotationInputPerformed;
            inputsystem.Tank.TowerRotation.canceled += OnTowerRotationInputPerformed;
            inputsystem.Tank.Fire.performed += OnFireInputPerformed;
            inputsystem.Tank.Fire.canceled += OnFireInputPerformed;
        }
        else
        {
            inputsystem.Tank.TowerRotation.performed -= OnTowerRotationInputPerformed;
            inputsystem.Tank.TowerRotation.canceled -= OnTowerRotationInputPerformed;
            inputsystem.Tank.Fire.performed -= OnFireInputPerformed;
            inputsystem.Tank.Fire.canceled -= OnFireInputPerformed;
        }
    }

    private void OnTowerRotationInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isRotating = context.performed;
        towerRotation = context.ReadValue<float>();
    }

    private void OnFireInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isFiring = context.performed;
    }

    private void Update()
    {
        if (!localTankInfo.Player.IsLocalPlayer)
        {
            return;
        }

        MovementInput();
        TryRotateTower();
        TryFireGrenade();
    }

    private void MovementInput()
    {
        Vector2 inputVector = inputsystem.Tank.Movement.ReadValue<Vector2>();

        currentMovementInputVector = Vector2.SmoothDamp(currentMovementInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);

        currentMovementInputVector.y = Mathf.Approximately(inputVector.y, 0f) ? 0 : currentMovementInputVector.y;

        Client.Singleton.SendToServer(new NetTInput(localTankInfo.Player.ID, currentMovementInputVector.x, currentMovementInputVector.y));
    }

    private void TryRotateTower()
    {
        if (isRotating)
        {
            Client.Singleton.SendToServer(new NetTTowerInput(localTankInfo.Player.ID, towerRotation));
        }
    }

    private void TryFireGrenade()
    {
        if (isFiring)
        {
            Client.Singleton.SendToServer(new NetTFireInput(localTankInfo.Player.ID, Vector3.zero, 0));
        }
    }
}
