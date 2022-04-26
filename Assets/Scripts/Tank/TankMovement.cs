using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private Rigidbody rb;
    private TNetwork tNetWork;
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 90f;

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        this.tNetWork = GetComponent<TNetwork>();
    }

    void Update()
    {
        if (this.tNetWork.IsOwner)
            this.PlayerInput();
    }

    private void FixedUpdate()
    {
        if (this.tNetWork.IsOwner)
            this.Move();
    }

    private void Move()
    {
        Vector3 prePosition = rb.position;

        this.rb.MovePosition(transform.position + transform.forward * this.verticalInput * this.moveSpeed * Time.fixedDeltaTime);
        this.rb.MoveRotation(transform.rotation * Quaternion.Euler(Vector3.up * this.horizontalInput * this.rotationSpeed * Time.fixedDeltaTime));

        if (Vector3.Distance(prePosition, rb.position) >= 0.01f)
            Client.Singleton.SendToServer(new NetTMove(this.tNetWork.ID, this.rb.position, this.rb.rotation));
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
    }
}
