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

    [SerializeField]
    private float smoothInputSpeed = .1f;

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
            inputsystem.Tank.TowerRotation.canceled += OnTowerRotationInputPerformed;
            inputsystem.Tank.Fire.performed += OnFireInputPerformed;
        }
    }

    private void OnTowerRotationInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isRotating = context.performed;
        towerRotation = context.ReadValue<float>();
        Client.Singleton.SendToServer(new NetTTowerInput(localTankInfo.ID, towerRotation));
    }

    private void OnFireInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }

    private void Update()
    {
        if (!localTankInfo.IsLocalPlayer)
        {
            return;
        }

        MovementInput();
        TryRotateTower();
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
            Client.Singleton.SendToServer(new NetTInput(localTankInfo.ID, currentMovementInputVector.x, currentMovementInputVector.y));
        }
    }

    private void TryRotateTower()
    {
        if (isRotating)
        {
            Client.Singleton.SendToServer(new NetTTowerInput(localTankInfo.ID, towerRotation));
        }
    }
}
