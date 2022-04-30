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

    // for tank movement
    private Vector2 currentMovementInputVector;
    private Vector2 smoothInputVelocity;

    // for tower rotation
    private float currentRotationInput;
    private float currentRotationVelocity;

    // Interpolators
    private int totalInterpolateStep = 10;
    private FloatInterpolator floatInterpolator;
    private Vector3Interpolator vector3Interpolator;
    private QuaternionInterpolator quaternionInterpolator;

    [SerializeField] private float smoothInputSpeed = .1f;

    public GameObject TankTower;

    private void Awake()
    {
        localRb = GetComponent<Rigidbody>();
        localTankInfo = GetComponent<TankInformation>();
        inputsystem = InputEventManager.Singleton.Inputsystem;
        floatInterpolator = new FloatInterpolator(totalInterpolateStep);
        vector3Interpolator = new Vector3Interpolator(totalInterpolateStep);
        quaternionInterpolator = new QuaternionInterpolator(totalInterpolateStep);
    }

    private void Start()
    {
        registerToEvent(true);

    }

    void FixedUpdate()
    {
        if (localTankInfo.IsLocalPlayer)
        {
            TankMovementInput();
            TankTowerInput();
        }
    }

    private void TankTowerInput()
    {
        float inputFloat = inputsystem.Tank.TowerRotation.ReadValue<float>();

        currentRotationInput = floatInterpolator.Interpolate(currentRotationInput, inputFloat);
        if (currentRotationInput != 0)
            Client.Singleton.SendToServer(new NetTTowerInput(localTankInfo.ID, currentRotationInput));
    }

    private void TankMovementInput()
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
            // Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, currentMovementInputVector.x, currentMovementInputVector.y));
            Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, inputVector.x, inputVector.y)); //Input Raw without smoothing
        }
    }


    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_TRANSFORM += OnClientReceivedTMoveMessage;
            NetUtility.C_T_TOWER_ROTATION += OnClientReceivedTTowerRotation;
        }
        else
        {
            NetUtility.C_T_TRANSFORM -= OnClientReceivedTMoveMessage;
            NetUtility.C_T_TOWER_ROTATION -= OnClientReceivedTTowerRotation;
        }
    }

    private void OnClientReceivedTTowerRotation(NetMessage message)
    {
        NetTTowerRotation tTowerRotationMessage = message as NetTTowerRotation;

        if (localTankInfo.ID != tTowerRotationMessage.ID) return;

        TankTower.transform.rotation = tTowerRotationMessage.Rotation;
    }

    private void OnClientReceivedTMoveMessage(NetMessage message)
    {
        NetTTransform tTransformMessage = message as NetTTransform;

        if (localTankInfo.ID != tTransformMessage.ID) return;

        Move(tTransformMessage.Position, tTransformMessage.Rotation);
    }

    private void Move(Vector3 position, Quaternion rotation)
    {
        localRb.transform.position = vector3Interpolator.Interpolate(localRb.transform.position, position);
        localRb.transform.rotation = quaternionInterpolator.Interpolate(localRb.transform.rotation, rotation);
    }
}
