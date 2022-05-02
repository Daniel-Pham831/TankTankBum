using System;
using UnityEngine;

public class TankAnimationHandler : MonoBehaviour
{
    [SerializeField]
    private TankInformation localTankInfo;

    [SerializeField]
    private Animator tankWheelAnimator;

    // [SerializeField]
    // private Animator tankFireAnimator;

    private void Start()
    {
        RegisterToEvent(true);
    }

    private void RegisterToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_INPUT += OnClientReceivedTInputMessage;
            NetUtility.C_T_FIRE_INPUT += OnClientReceivedTFireInputMessage;
        }
        else
        {
            NetUtility.C_T_INPUT -= OnClientReceivedTInputMessage;
            NetUtility.C_T_FIRE_INPUT -= OnClientReceivedTFireInputMessage;
        }
    }

    private void OnClientReceivedTFireInputMessage(NetMessage message)
    {
        Debug.Log($"Client {(message as NetTFireInput).FireDirection}");
    }

    private void OnClientReceivedTInputMessage(NetMessage message)
    {
        NetTInput tInputMessage = message as NetTInput;

        if (localTankInfo.ID != tInputMessage.ID) return;

        tankWheelAnimator.SetFloat("X", tInputMessage.HorizontalInput);
        tankWheelAnimator.SetFloat("Y", tInputMessage.VerticalInput);
    }
}
