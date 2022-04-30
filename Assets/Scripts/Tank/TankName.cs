using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TankName : MonoBehaviour
{
    private GameObject tank;
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private Color myTeamNameColor;
    [SerializeField] private Color otherTeamNameColor;

    private void Update()
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
        Debug.Log("set");
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
