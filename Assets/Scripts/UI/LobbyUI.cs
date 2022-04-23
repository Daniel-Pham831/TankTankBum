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
    [SerializeField] private GameObject readyOrStartBtn;

    public Action<Player> OnPlayerJoinedSlot;
    public Action<Player> OnPlayerExitedSlot;
    public Action<Team, byte, SlotState> OnSlotStateChanged;
    public Action<Team, byte> OnSlotReadyOrStartPress;
    public Action<Team, byte> OnSlotReset;
    public Action OnAllSlotReset;

    public Action OnLobbyLeft;


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
            ClientInformation.Singleton.OnNewJoinedPlayer += OnNewJoinedPlayer;
            ClientInformation.Singleton.OnDisconnectedClient += OnDisconnectedClient;
            ClientInformation.Singleton.OnDeclareHost += OnDeclareHost;

            Client.Singleton.OnServerDisconnect += OnLeaveBtn;
        }
        else
        {
            ClientInformation.Singleton.OnNewJoinedPlayer -= OnNewJoinedPlayer;
            ClientInformation.Singleton.OnDisconnectedClient -= OnDisconnectedClient;
            ClientInformation.Singleton.OnDeclareHost -= OnDeclareHost;

            Client.Singleton.OnServerDisconnect += OnLeaveBtn;
        }
    }

    public void OnStartOrReadyBtn()
    {
        this.OnSlotReadyOrStartPress?.Invoke(ClientInformation.Singleton.MyPlayerInformation.Team, ClientInformation.Singleton.MyPlayerInformation.SlotIndex);
    }

    public void OnLeaveBtn()
    {
        //backend
        if (ClientInformation.Singleton.IsHost)
            Server.Singleton.Shutdown();
        else
            Client.Singleton.Shutdown();

        //frontend
        this.OnLobbyLeft?.Invoke();
        this.OnAllSlotReset?.Invoke();
    }

    private void OnDeclareHost(bool isHost)
    {
        TMP_Text readyOrStartBtnName = this.readyOrStartBtn.GetComponentInChildren<TMP_Text>();
        readyOrStartBtnName.SetText(isHost ? "StartGame" : "Ready");
    }

    private void OnDisconnectedClient(Player disconnectedPlayer)
    {
        this.OnPlayerExitedSlot?.Invoke(disconnectedPlayer);
    }

    private void OnNewJoinedPlayer(Player joinedPlayer)
    {
        this.OnPlayerJoinedSlot?.Invoke(joinedPlayer);
    }
}