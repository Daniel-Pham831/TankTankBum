using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SlotState
{
    Empty = 0,
    Occupied = 1,
    OwnerOccupied = 2
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
    [SerializeField] private Color ownerOccupiedSlotColor;
    [SerializeField] private Image nameBackgroundColor;

    [SerializeField] private Color emptyBackground;
    [SerializeField] private Color blueBackground;
    [SerializeField] private Color redBackground;

    [SerializeField] private Image slotBackgroundColor;


    private bool IsThisSlot(byte slotIndex)
    {
        return SlotIndex == slotIndex;
    }

    private bool IsOwnerSlot(byte slotIndex)
    {
        SlotPlayerInformation ownerInformation = ClientInformation.Singleton.MyPlayerInformation;

        return ownerInformation.SlotIndex == slotIndex;
    }

    private void ResetSlot(byte slotIndex)
    {
        if (IsThisSlot(slotIndex))
        {
            ResetSlot();
        }
    }
    private void ResetSlot()
    {
        slotReadyToggle.isOn = false;
        SetSlotStage(SlotState.Empty);
        slotBackgroundColor.color = emptyBackground;
        slotName.SetText("");
    }

    private void SetSlotStage(SlotState state)
    {
        slotState = state;

        Color color = emptySlotColor;
        switch (state)
        {
            case SlotState.Empty:
                color = emptySlotColor;
                break;

            case SlotState.Occupied:
                color = occupiedSlotColor;
                break;

            case SlotState.OwnerOccupied:
                color = ownerOccupiedSlotColor;
                break;
        }

        nameBackgroundColor.color = color;
    }

    private void SetSlotInformationBasedOf(SlotPlayerInformation player)
    {
        if (IsThisSlot(player.SlotIndex))
        {
            slotName.SetText(player.Name);
            SlotTeam = player.Team;
            SetSlotTeamColor();
            SetSlotReadyState(player.ReadyState);

            if (IsOwnerSlot(player.SlotIndex))
                SetSlotStage(SlotState.OwnerOccupied);
            else
                SetSlotStage(SlotState.Occupied);
        }
    }

    private void SetSlotReadyState(ReadyState readyState)
    {
        slotReadyToggle.isOn = readyState == ReadyState.Ready ? true : false;
    }

    private void SetSlotTeamColor()
    {
        slotBackgroundColor.color = SlotTeam == Team.Blue ? blueBackground : redBackground;
    }

    #region EventMethods
    private void Start()
    {
        registerToEvent(true);

        ResetSlot();
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            LobbyUI.Singleton.OnPlayerJoinedSlot += OnPlayerJoinedSlot;
            LobbyUI.Singleton.OnAllSlotReset += OnAllSlotReset;
            LobbyUI.Singleton.OnSlotReset += OnSlotReset;
            LobbyUI.Singleton.OnSlotStateChanged += OnSlotStateChanged;
            LobbyUI.Singleton.OnPlayerExitedSlot += OnPlayerExitedSlot;
            LobbyUI.Singleton.OnSlotReadyOrStartPress += OnSlotReadyOrStartPress;
            LobbyUI.Singleton.OnSlotSwitchTeam += OnSlotSwitchTeam;
        }
        else
        {
            LobbyUI.Singleton.OnPlayerJoinedSlot -= OnPlayerJoinedSlot;
            LobbyUI.Singleton.OnAllSlotReset -= OnAllSlotReset;
            LobbyUI.Singleton.OnSlotReset -= OnSlotReset;
            LobbyUI.Singleton.OnSlotStateChanged -= OnSlotStateChanged;
            LobbyUI.Singleton.OnPlayerExitedSlot -= OnPlayerExitedSlot;
            LobbyUI.Singleton.OnSlotReadyOrStartPress -= OnSlotReadyOrStartPress;
            LobbyUI.Singleton.OnSlotSwitchTeam -= OnSlotSwitchTeam;
        }
    }

    private void OnSlotSwitchTeam(byte slotIndex)
    {
        if (IsThisSlot(slotIndex))
        {
            SlotTeam = Team.Blue == SlotTeam ? Team.Red : Team.Blue;
            SetSlotTeamColor();
        }
    }

    private void OnSlotReadyOrStartPress(byte slotIndex, ReadyState readyState)
    {
        if (IsThisSlot(slotIndex))
        {
            SetSlotReadyState(readyState);
        }
    }

    private void OnPlayerExitedSlot(SlotPlayerInformation exitedPlayer)
    {
        ResetSlot(exitedPlayer.SlotIndex);
    }

    private void OnPlayerJoinedSlot(SlotPlayerInformation joinedPlayer)
    {
        SetSlotInformationBasedOf(joinedPlayer);
    }

    private void OnAllSlotReset()
    {
        ResetSlot(SlotIndex);
    }

    private void OnSlotReset(byte slotIndex)
    {
        ResetSlot(slotIndex);
    }

    private void OnSlotStateChanged(byte slotIndex, SlotState slotState)
    {
        if (IsThisSlot(slotIndex))
        {
            SetSlotStage(slotState);
        }
    }
    #endregion

}
