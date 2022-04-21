using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Singleton { get; private set; }
    [SerializeField] private GameObject[] blueSlots;
    [SerializeField] private GameObject[] redSlots;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }

    private void Start()
    {
        foreach (GameObject slot in this.blueSlots)
        {
            slot.GetComponentInChildren<Toggle>().isOn = false;
            slot.GetComponentInChildren<TMP_Text>().SetText("");
        }

        foreach (GameObject slot in this.redSlots)
        {
            slot.GetComponentInChildren<Toggle>().isOn = false;
            slot.GetComponentInChildren<TMP_Text>().SetText("");
        }

        this.registerToEvent(true);
    }

    private void OnDestroy()
    {
        Singleton = null;
        this.registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            PlayerInformation.Singleton.OnNewJoinedPlayer += OnNewJoinedPlayer;
        }
        else
        {
            PlayerInformation.Singleton.OnNewJoinedPlayer -= OnNewJoinedPlayer;

        }
    }

    private void OnNewJoinedPlayer(Player joinedPlayer)
    {
        GameObject joinedSlot;
        if (joinedPlayer.Team == Team.Blue)
        {
            joinedSlot = this.blueSlots[joinedPlayer.SlotIndex];
        }
        else
        {
            joinedSlot = this.redSlots[joinedPlayer.SlotIndex];
        }

        joinedSlot.GetComponentInChildren<TMP_Text>().SetText(joinedPlayer.Name);
    }
}