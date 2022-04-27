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

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        this.localTankInfo = GetComponent<TankInformation>();
    }

    private void Start()
    {
        this.registerToEvent(true);
    }

    void Update()
    {
        if (this.localTankInfo.IsLocalPlayer)
            this.PlayerInput();
    }

    private void Move()
    {
        Vector3 prePosition = rb.position;

        this.rb.MovePosition(transform.position + transform.forward * this.verticalInput * this.moveSpeed * Time.fixedDeltaTime);
        this.rb.MoveRotation(transform.rotation * Quaternion.Euler(Vector3.up * this.horizontalInput * this.rotationSpeed * Time.fixedDeltaTime));

        if (Vector3.Distance(prePosition, rb.position) >= 0.01f)
            Client.Singleton.SendToServer(new NetTTransform(this.localTankInfo.ID, this.rb.position, this.rb.rotation));
    }

    private void PlayerInput()
    {
        this.horizontalInput = Input.GetAxis("Horizontal");
        this.verticalInput = Input.GetAxis("Vertical");

        // Should self-made a smooth input here

        if (this.verticalInput != 0)
        {
            if (this.verticalInput < 0)
            {
                this.horizontalInput *= -1;
            }
        }

        if (new Vector2(this.horizontalInput, this.verticalInput) != Vector2.zero)
            Client.Singleton.SendToServer(new NetTInput(this.localTankInfo.ID, this.horizontalInput, this.verticalInput));
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

        this.Move(tMoveMessage.Position, tMoveMessage.Rotation);
    }

    private void Move(Vector3 position, Quaternion Rotaion)
    {
        transform.position = position;
        transform.rotation = Rotaion;
    }

}
