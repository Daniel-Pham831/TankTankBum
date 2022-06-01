using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TankName : MonoBehaviour
{
    private readonly float timeToDestroyAfterTankDie = 1;
    private GameObject tank;
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private Color myTeamNameColor;
    [SerializeField] private Color otherTeamNameColor;

    private void LateUpdate()
    {
        if (tank != null)
        {
            transform.position = tank.transform.position;
        }
    }

    public void SetUpTankName(GameObject tankToFollow, string tankName)
    {
        tank = tankToFollow;
        NameText.SetText(tankName);

        if (tank.TryGetComponent<TankHealth>(out TankHealth tankHealth))
        {
            tankHealth.OnDie += OnTankDie;
        }
    }

    private void OnTankDie()
    {

        if (tank.TryGetComponent<TankHealth>(out TankHealth tankHealth))
        {
            tankHealth.OnDie -= OnTankDie;

            Destroy(this.gameObject, timeToDestroyAfterTankDie);
        }
    }

    public void SetNameRot(Quaternion rot)
    {
        transform.rotation = rot;
    }

    public void SetNameColor(Team localPlayerTeam, Team thisTankTeam)
    {
        Color targetColor = localPlayerTeam == thisTankTeam ? myTeamNameColor : otherTeamNameColor;

        NameText.color = targetColor;
    }
}
