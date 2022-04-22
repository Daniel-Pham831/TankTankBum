using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SlotState
{
    Empty = 0,
    Occupied = 1
}

public class Slot : MonoBehaviour
{
    public Team SlotTeam { set; get; }
    public byte SlotIndex { set; get; }

    [SerializeField] private Toggle slotReadyToggle;
    [SerializeField] private TMP_Text slotName;
    [SerializeField] private SlotState slotState;


    [SerializeField] private Color emptySlotColor;
    [SerializeField] private Color occupiedSlotColor;
    [SerializeField] private Image slotColor;


    private bool IsThisSlot(Team slotTeam, byte slotIndex)
    {
        return this.SlotTeam == slotTeam && this.SlotIndex == slotIndex;
    }

    private void ResetSlot(Team slotTeam, byte slotIndex)
    {
        if (this.IsThisSlot(slotTeam, slotIndex))
        {
            this.ResetSlot();
        }
    }
    private void ResetSlot()
    {
        this.slotReadyToggle.isOn = false;
        this.SetSlotStage(SlotState.Empty);
        this.slotName.SetText("");
    }

    private void SetSlotStage(SlotState state)
    {
        this.slotState = state;
        this.slotColor.color = this.slotState == SlotState.Empty ? this.emptySlotColor : this.occupiedSlotColor;
    }

    private void SetSlotInformationBasedOf(Player player)
    {
        if (this.IsThisSlot(player.Team, player.SlotIndex))
        {
            this.slotName.SetText(player.Name);
            this.SetSlotStage(SlotState.Occupied);
        }
    }

    #region EventMethods
    private void Start()
    {
        this.registerToEvent(true);

        this.ResetSlot();
    }

    private void OnDestroy()
    {
        this.registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            LobbyUI.Singleton.OnPlayerJoinedSlot += OnPlayerJoinedSlot;
            LobbyUI.Singleton.OnAllSlotReset += OnAllSlotReset;
            LobbyUI.Singleton.OnSlotReset += OnSlotReset;
            LobbyUI.Singleton.OnSlotStateChanged += OnSlotStateChanged;
        }
        else
        {
            LobbyUI.Singleton.OnPlayerJoinedSlot -= OnPlayerJoinedSlot;
            LobbyUI.Singleton.OnAllSlotReset -= OnAllSlotReset;
            LobbyUI.Singleton.OnSlotReset -= OnSlotReset;
            LobbyUI.Singleton.OnSlotStateChanged -= OnSlotStateChanged;
        }
    }

    private void OnPlayerJoinedSlot(Player player)
    {
        this.SetSlotInformationBasedOf(player);
    }

    private void OnAllSlotReset()
    {
        this.ResetSlot(this.SlotTeam, this.SlotIndex);
    }

    private void OnSlotReset(Team slotTeam, byte slotIndex)
    {
        this.ResetSlot(slotTeam, slotIndex);
    }

    private void OnSlotStateChanged(Team slotTeam, byte slotIndex, SlotState slotState)
    {
        if (this.IsThisSlot(slotTeam, slotIndex))
        {
            this.SetSlotStage(slotState);
        }
    }
    #endregion

}
