using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 90f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.PlayerInput();
    }

    private void FixedUpdate()
    {
        this.Move();

    }

    private void Move()
    {
        rb.MovePosition(transform.position + transform.forward * verticalInput * moveSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(transform.rotation * Quaternion.Euler(Vector3.up * horizontalInput * rotationSpeed * Time.fixedDeltaTime));
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput != 0)
        {
            if (verticalInput < 0)
            {
                horizontalInput *= -1;
            }
        }
    }
}
