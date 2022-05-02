using System;
using UnityEngine;

public class TankFire : MonoBehaviour
{

    [SerializeField]
    private TankInformation localTankInfo;

    private void Start()
    {
        RegisterToEvent(true);
    }

    private void RegisterToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_FIRE_INPUT += OnClientReceivedTFireInputMessage;
        }
        else
        {
            NetUtility.C_T_FIRE_INPUT -= OnClientReceivedTFireInputMessage;
        }
    }

    private void OnClientReceivedTFireInputMessage(NetMessage message)
    {
        throw new NotImplementedException();
    }
}
