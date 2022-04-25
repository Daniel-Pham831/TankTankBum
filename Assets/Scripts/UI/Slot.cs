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
        return this.SlotIndex == slotIndex;
    }

    private bool IsOwnerSlot(byte slotIndex)
    {
        Player ownerInformation = ClientInformation.Singleton.MyPlayerInformation;

        return ownerInformation.SlotIndex == slotIndex;
    }

    private void ResetSlot(byte slotIndex)
    {
        if (this.IsThisSlot(slotIndex))
        {
            this.ResetSlot();
        }
    }
    private void ResetSlot()
    {
        this.slotReadyToggle.isOn = false;
        this.SetSlotStage(SlotState.Empty);
        this.slotBackgroundColor.color = emptyBackground;
        this.slotName.SetText("");
    }

    private void SetSlotStage(SlotState state)
    {
        this.slotState = state;

        Color color = this.emptySlotColor;
        switch (state)
        {
            case SlotState.Empty:
                color = this.emptySlotColor;
                break;

            case SlotState.Occupied:
                color = this.occupiedSlotColor;
                break;

            case SlotState.OwnerOccupied:
                color = this.ownerOccupiedSlotColor;
                break;
        }

        this.nameBackgroundColor.color = color;
    }

    private void SetSlotInformationBasedOf(Player player)
    {
        if (this.IsThisSlot(player.SlotIndex))
        {
            this.slotName.SetText(player.Name);
            this.SlotTeam = player.Team;
            this.SetSlotTeamColor();
            this.SetSlotReadyState(player.ReadyState);

            if (this.IsOwnerSlot(player.SlotIndex))
                this.SetSlotStage(SlotState.OwnerOccupied);
            else
                this.SetSlotStage(SlotState.Occupied);
        }
    }

    private void SetSlotReadyState(ReadyState readyState)
    {
        this.slotReadyToggle.isOn = readyState == ReadyState.Ready ? true : false;
    }

    private void SetSlotTeamColor()
    {
        this.slotBackgroundColor.color = this.SlotTeam == Team.Blue ? this.blueBackground : this.redBackground;
    }

    #region EventMethods
    private void Start()
    {
        this.registerToEvent(true);

        this.ResetSlot();
    }

    private void OnDestroy()
    {
        //  this.registerToEvent(false);
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
        if (this.IsThisSlot(slotIndex))
        {
            this.SlotTeam = Team.Blue == this.SlotTeam ? Team.Red : Team.Blue;
            this.SetSlotTeamColor();
        }
    }

    private void OnSlotReadyOrStartPress(byte slotIndex, ReadyState readyState)
    {
        if (this.IsThisSlot(slotIndex))
        {
            this.SetSlotReadyState(readyState);
        }
    }

    private void OnPlayerExitedSlot(Player exitedPlayer)
    {
        this.ResetSlot(exitedPlayer.SlotIndex);
    }

    private void OnPlayerJoinedSlot(Player joinedPlayer)
    {
        this.SetSlotInformationBasedOf(joinedPlayer);
    }

    private void OnAllSlotReset()
    {
        this.ResetSlot(this.SlotIndex);
    }

    private void OnSlotReset(byte slotIndex)
    {
        this.ResetSlot(slotIndex);
    }

    private void OnSlotStateChanged(byte slotIndex, SlotState slotState)
    {
        if (this.IsThisSlot(slotIndex))
        {
            this.SetSlotStage(slotState);
        }
    }
    #endregion

}
