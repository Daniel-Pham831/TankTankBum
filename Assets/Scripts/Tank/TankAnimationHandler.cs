using System;
using UnityEngine;

public class TankAnimationHandler : MonoBehaviour
{
    [SerializeField]
    private TankInformation localTankInfo;

    [SerializeField]
    private Animator tankWheelAnimator;

    private void Start()
    {
        RegisterToEvent(true);
    }

    private void OnDestroy()
    {
        RegisterToEvent(false);
    }

    private void RegisterToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_T_INPUT += OnClientReceivedTInputMessage;
        }
        else
        {
            NetUtility.C_T_INPUT -= OnClientReceivedTInputMessage;
        }
    }

    private void OnClientReceivedTInputMessage(NetMessage message)
    {
        NetTInput tInputMessage = message as NetTInput;

        if (localTankInfo.Player.ID != tInputMessage.ID) return;

        tankWheelAnimator.SetFloat("X", tInputMessage.HorizontalInput);
        tankWheelAnimator.SetFloat("Y", tInputMessage.VerticalInput);
    }
}
