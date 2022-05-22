using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is for handling tank movement
*/
public class TankMovement : MonoBehaviour
{
    private TankInformation localTankInfo;

    private Rigidbody localRb;

    public GameObject TankTower;
    private float smoothTime = 10f;

    private void Awake()
    {
        localRb = GetComponent<Rigidbody>();
        localTankInfo = GetComponent<TankInformation>();
    }

    private void Start()
    {
        registerToEvent(true);
    }

    private void OnDestroy()
    {
        registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_TOWER_ROTATION += OnClientReceivedTTowerRotationMessage;
            NetUtility.C_T_VELOCITY += OnClientReceivedTVelocityMessage;
            NetUtility.C_T_POSITION += OnClientReceivedTPositionMessage;
            NetUtility.C_T_ROTATION += OnClientReceivedTRotationMessage;
        }
        else
        {
            NetUtility.C_T_TOWER_ROTATION -= OnClientReceivedTTowerRotationMessage;
            NetUtility.C_T_VELOCITY -= OnClientReceivedTVelocityMessage;
            NetUtility.C_T_POSITION -= OnClientReceivedTPositionMessage;
            NetUtility.C_T_ROTATION -= OnClientReceivedTRotationMessage;
        }
    }

    private void OnClientReceivedTPositionMessage(NetMessage message)
    {
        NetTPosition tPositionMessage = message as NetTPosition;

        if (localTankInfo.Player.ID != tPositionMessage.ID) return;

        localRb.transform.position = Vector3.Lerp(localRb.transform.position, tPositionMessage.Position, Time.deltaTime * smoothTime);
    }

    private void OnClientReceivedTRotationMessage(NetMessage message)
    {
        NetTRotation tRotationMessage = message as NetTRotation;

        if (localTankInfo.Player.ID != tRotationMessage.ID) return;

        localRb.transform.forward = Vector3.Lerp(localRb.transform.forward, tRotationMessage.Forward, Time.deltaTime * smoothTime);
    }

    private void OnClientReceivedTVelocityMessage(NetMessage message)
    {
        NetTVelocity tVelocityMessage = message as NetTVelocity;

        if (localTankInfo.Player.ID != tVelocityMessage.ID) return;

        localRb.velocity = tVelocityMessage.Velocity;
    }

    private void OnClientReceivedTTowerRotationMessage(NetMessage message)
    {
        NetTTowerRotation tTowerRotationMessage = message as NetTTowerRotation;

        if (localTankInfo.Player.ID != tTowerRotationMessage.ID) return;

        TankTower.transform.localEulerAngles = tTowerRotationMessage.LocalEulerAngles;
    }
}
