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


    // Interpolators
    private int totalInterpolateStep = 10;
    private FloatInterpolator floatInterpolator;

    // for tank movement
    private Vector2 currentMovementInputVector;
    private Vector2 smoothInputVelocity;

    // for tower rotation
    private float currentRotationInput;
    private float currentRotationVelocity;

    private Vector3Interpolator vector3Interpolator;

    [SerializeField] private float smoothInputSpeed = .1f;


    private void Awake()
    {
        localTankInfo = GetComponent<TankInformation>();
        inputsystem = InputEventManager.Singleton.Inputsystem;
        inputsystem.Tank.Fire.performed += OnFireInputPerFormed;

        floatInterpolator = new FloatInterpolator(totalInterpolateStep);
    }

    void Update()
    {
        if (localTankInfo.IsLocalPlayer)
        {
            TankTowerInput();
            TankMovementInput();
        }
    }

    private void OnFireInputPerFormed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log(context);
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
            Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, currentMovementInputVector.x, currentMovementInputVector.y)); //Has smoothing effect
            // Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, inputVector.x, inputVector.y)); //Input Raw without smoothing
        }
    }
}
