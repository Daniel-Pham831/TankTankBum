using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNetwork : MonoBehaviour
{
    public byte ID;
    public Team Team;
    public bool IsOwner;
    public bool IsHost;
    private Rigidbody rb;

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_TMOVE += OnClientReceivedTMoveMessage;
        }
        else
        {
            NetUtility.C_TMOVE -= OnClientReceivedTMoveMessage;
        }
    }

    private void OnClientReceivedTMoveMessage(NetMessage message)
    {
        Debug.Log("Received TMove");
        NetTMove tMoveMessage = message as NetTMove;

        if (this.ID != tMoveMessage.ID) return;

        this.Move(tMoveMessage.Position, tMoveMessage.Rotation);
    }

    private void Move(Vector3 position, Quaternion Rotaion)
    {
        transform.position = position;
        transform.rotation = Rotaion;
    }
}
