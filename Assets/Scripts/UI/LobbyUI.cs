using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Singleton { get; private set; }
    [SerializeField] private GameObject blueSlotRoot;
    [SerializeField] private GameObject redSlotRoot;
    [SerializeField] private GameObject slotPrefab;

    public Action<Player> OnPlayerJoinedSlot;
    public Action<Team, byte, SlotState> OnSlotStateChanged;
    public Action<Team, byte> OnSlotReset;
    public Action OnAllSlotReset;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }

    private void Start()
    {
        this.registerToEvent(true);

        this.GenerateAllSlots();
    }

    private void GenerateAllSlots()
    {
        for (int i = 0; i < (GameInformation.Singleton.MaxPlayer / 2) + 1; i++)
        {
            this.GenerateOneSlot(Team.Blue, (byte)i);
            this.GenerateOneSlot(Team.Red, (byte)i);
            this.OnSlotStateChanged?.Invoke(Team.Blue, (byte)i, SlotState.Empty);
            this.OnSlotStateChanged?.Invoke(Team.Red, (byte)i, SlotState.Empty);
        }
    }

    private void GenerateOneSlot(Team slotTeam, byte slotIndex)
    {
        GameObject newSlot = Instantiate(this.slotPrefab, slotTeam == Team.Blue ? this.blueSlotRoot.transform : this.redSlotRoot.transform);
        newSlot.name = $"{slotTeam} Slot: Index {slotIndex}";
        Slot slotInfo = newSlot.GetComponent<Slot>();
        slotInfo.SlotTeam = slotTeam;
        slotInfo.SlotIndex = slotIndex;
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
            PlayerInformation.Singleton.OnDisconnectedSlot += OnDisconnectedClient;
            MainMenuUI.Singleton.OnLobbyLeft += OnLobbyLeft;

            Client.Singleton.OnServerShutDown += OnServerShutDown;
        }
        else
        {
            PlayerInformation.Singleton.OnNewJoinedPlayer -= OnNewJoinedPlayer;
            PlayerInformation.Singleton.OnDisconnectedSlot -= OnDisconnectedClient;
            MainMenuUI.Singleton.OnLobbyLeft -= OnLobbyLeft;

            Client.Singleton.OnServerShutDown -= OnServerShutDown;
        }
    }

    private void OnLobbyLeft()
    {
        this.OnAllSlotReset?.Invoke();
    }

    private void OnServerShutDown()
    {
        this.OnAllSlotReset?.Invoke();
    }

    private void OnDisconnectedClient(Team team, byte id)
    {
        this.OnSlotReset?.Invoke(team, id);
    }

    private void OnNewJoinedPlayer(Player joinedPlayer)
    {
        this.OnPlayerJoinedSlot?.Invoke(joinedPlayer);
    }
}