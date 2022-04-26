using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 upDownDirection;
    private float moveSpeed = 10f;
    private float gravityValue = -9.81f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.PlayerInput();
        this.Move();
    }

    private void Move()
    {

    }

    private void PlayerInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        upDownDirection = new Vector3(0, 0, z);
    }
}
