using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Singleton { get; private set; }
    [SerializeField] private GameObject leftSlotRoot;
    [SerializeField] private GameObject rightSlotRoot;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject readyOrStartBtn;

    public Action<Player> OnPlayerJoinedSlot;
    public Action<Player> OnPlayerExitedSlot;
    public Action<byte, SlotState> OnSlotStateChanged;
    public Action<byte> OnSlotReadyOrStartPress;
    public Action<byte> OnSlotReset;
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
        for (int i = 0; i < GameInformation.Singleton.MaxPlayer; i++)
        {
            this.GenerateOneSlot((byte)i);
            this.OnSlotStateChanged?.Invoke((byte)i, SlotState.Empty);
        }
    }

    private void GenerateOneSlot(byte slotIndex)
    {
        GameObject newSlot = Instantiate(this.slotPrefab, slotIndex % 2 == 0 ? this.leftSlotRoot.transform : this.rightSlotRoot.transform);
        newSlot.name = $"Slot: Index {slotIndex}";
        Slot slotInfo = newSlot.GetComponent<Slot>();
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
        this.OnSlotReadyOrStartPress?.Invoke(ClientInformation.Singleton.MyPlayerInformation.SlotIndex);
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