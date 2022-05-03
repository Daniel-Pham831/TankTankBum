using System;
using UnityEngine;

public class TankFire : MonoBehaviour
{

    [SerializeField]
    private TankInformation localTankInfo;
    [SerializeField] private GameObject firePosition;


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
        NetTFireInput tFireInputMessage = message as NetTFireInput;

        if (localTankInfo.ID != tFireInputMessage.ID) return;

        GetGrenadeAndFireIt(tFireInputMessage);
    }

    private void GetGrenadeAndFireIt(NetTFireInput tFireInputMessage)
    {
        Pool tankGrenadePool = ObjectPoolManager.Singleton.GetPool(PoolType.TGrenade);
        GameObject tankGrenade = tankGrenadePool.Get();

        TankGrenadeMovement tankGrenadeMovement = tankGrenade.GetComponent<TankGrenadeMovement>();
        tankGrenadeMovement.FireAtDirection(tFireInputMessage.FireDirection, firePosition.transform.position, 20);

        GrenadeInformation grenadeInformation = tankGrenade.GetComponent<GrenadeInformation>();
        grenadeInformation.ID = localTankInfo.ID;
        grenadeInformation.Team = localTankInfo.Team;
    }
}
