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

    private void Awake()
    {
        localRb = GetComponent<Rigidbody>();
        localTankInfo = GetComponent<TankInformation>();
    }

    private void Start()
    {
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            // NetUtility.C_T_TRANSFORM += OnClientReceivedTMoveMessage;
            NetUtility.C_T_TOWER_ROTATION += OnClientReceivedTTowerRotationMessage;
            NetUtility.C_T_VELOCITY += OnClientReceivedTVelocityMessage;
            NetUtility.C_T_POSITION += OnClientReceivedTPositionMessage;
            NetUtility.C_T_ROTATION += OnClientReceivedTRotationMessage;
        }
        else
        {
            // NetUtility.C_T_TRANSFORM -= OnClientReceivedTMoveMessage;
            NetUtility.C_T_TOWER_ROTATION -= OnClientReceivedTTowerRotationMessage;
            NetUtility.C_T_VELOCITY -= OnClientReceivedTVelocityMessage;
            NetUtility.C_T_POSITION -= OnClientReceivedTPositionMessage;
            NetUtility.C_T_ROTATION -= OnClientReceivedTRotationMessage;
        }
    }

    private void OnClientReceivedTPositionMessage(NetMessage message)
    {
        NetTPosition tPositionMessage = message as NetTPosition;

        if (localTankInfo.ID != tPositionMessage.ID) return;

        // localRb.transform.position = vector3Interpolator.Interpolate(localRb.transform.position, tPositionMessage.Position);
        localRb.MovePosition(tPositionMessage.Position);
    }

    private void OnClientReceivedTRotationMessage(NetMessage message)
    {
        NetTRotation tRotationMessage = message as NetTRotation;

        if (localTankInfo.ID != tRotationMessage.ID) return;

        localRb.MoveRotation(tRotationMessage.Rotation);
    }

    private void OnClientReceivedTVelocityMessage(NetMessage message)
    {
        NetTVelocity tVelocityMessage = message as NetTVelocity;

        if (localTankInfo.ID != tVelocityMessage.ID) return;

        localRb.velocity = tVelocityMessage.Velocity;
    }

    private void OnClientReceivedTTowerRotationMessage(NetMessage message)
    {
        NetTTowerRotation tTowerRotationMessage = message as NetTTowerRotation;

        if (localTankInfo.ID != tTowerRotationMessage.ID) return;

        TankTower.transform.rotation = tTowerRotationMessage.Rotation;
    }

    // private void OnClientReceivedTMoveMessage(NetMessage message)
    // {
    //     NetTTransform tTransformMessage = message as NetTTransform;

    //     if (localTankInfo.ID != tTransformMessage.ID) return;

    //     Move(tTransformMessage.Position, tTransformMessage.Rotation);
    // }

    // private void Move(Vector3 position, Quaternion rotation)
    // {
    //     // localRb.transform.position = vector3Interpolator.Interpolate(localRb.transform.position, position);
    //     // localRb.transform.rotation = quaternionInterpolator.Interpolate(localRb.transform.rotation, rotation);
    //     localRb.transform.position = position;
    //     localRb.transform.rotation = rotation;
    //     // localRb.MovePosition(position);
    //     // localRb.MoveRotation(rotation);
    // }
}
