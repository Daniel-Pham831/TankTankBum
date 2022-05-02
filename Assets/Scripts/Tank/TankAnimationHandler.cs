using UnityEngine;

public class TankAnimationHandler : MonoBehaviour
{
    [SerializeField]
    private TankInformation localTankInfo;

    [SerializeField]
    private Animator tankAnimator;

    private void Start()
    {
        RegisterToEvent(true);
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

        if (localTankInfo.ID != tInputMessage.ID) return;

        tankAnimator.SetFloat("X", tInputMessage.HorizontalInput);
        tankAnimator.SetFloat("Y", tInputMessage.VerticalInput);
    }
}
