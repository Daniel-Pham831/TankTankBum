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
    [SerializeField] private Image switchTeamBtn;
    [SerializeField] private Animator lobbyAnimator;

    public Action<SlotPlayerInformation> OnPlayerJoinedSlot;
    public Action<SlotPlayerInformation> OnPlayerExitedSlot;
    public Action<byte, SlotState> OnSlotStateChanged;
    public Action<byte, ReadyState> OnSlotReadyOrStartPress;
    public Action<byte> OnSlotReset;
    public Action<byte> OnSlotSwitchTeam;
    public Action OnAllSlotReset;

    public Action OnLobbyLeft;


    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }

    private void Start()
    {
        registerToEvent(true);

        GenerateAllSlots();
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            ClientInformation.Singleton.OnNewJoinedPlayer += OnNewJoinedPlayer;
            ClientInformation.Singleton.OnDisconnectedClient += OnDisconnectedClient;
            ClientInformation.Singleton.OnDeclareHost += OnDeclareHost;
            ClientInformation.Singleton.OnPlayerSwitchTeam += OnPlayerSwitchTeam;
            ClientInformation.Singleton.OnPlayerSwitchReadyState += OnPlayerSwitchReadyState;

            Client.Singleton.OnServerDisconnect += OnLeaveBtn;
        }
        else
        {
            ClientInformation.Singleton.OnNewJoinedPlayer -= OnNewJoinedPlayer;
            ClientInformation.Singleton.OnDisconnectedClient -= OnDisconnectedClient;
            ClientInformation.Singleton.OnDeclareHost -= OnDeclareHost;
            ClientInformation.Singleton.OnPlayerSwitchTeam -= OnPlayerSwitchTeam;
            ClientInformation.Singleton.OnPlayerSwitchReadyState -= OnPlayerSwitchReadyState;

            Client.Singleton.OnServerDisconnect -= OnLeaveBtn;
        }
    }

    private void OnPlayerSwitchReadyState(byte slotIndex, ReadyState readyState)
    {
        OnSlotReadyOrStartPress?.Invoke(slotIndex, readyState);
    }

    private void OnPlayerSwitchTeam(byte slotIndex)
    {
        OnSlotSwitchTeam?.Invoke(slotIndex);
    }

    private void GenerateAllSlots()
    {
        for (int i = 0; i < GameInformation.Singleton.MaxPlayer; i++)
        {
            GenerateOneSlot((byte)i);
            OnSlotStateChanged?.Invoke((byte)i, SlotState.Empty);
        }
    }

    private void GenerateOneSlot(byte slotIndex)
    {
        GameObject newSlot = Instantiate(slotPrefab, slotIndex % 2 == 0 ? leftSlotRoot.transform : rightSlotRoot.transform);
        newSlot.name = $"Slot: Index {slotIndex}";
        Slot slotInfo = newSlot.GetComponent<Slot>();
        slotInfo.SlotIndex = slotIndex;
    }

    public void OnStartOrReadyBtn()
    {
        SlotPlayerInformation MyPlayerInformation = ClientInformation.Singleton.MyPlayerInformation;

        // If the host press start btn
        if (MyPlayerInformation.IsHost)
        {
            if (!SlotPlayerInformation.HaveAllPlayersReadied(ClientInformation.Singleton.PlayerList))
            {
                // Pop up a READY error box indicating that can only start when all players are ready
                lobbyAnimator.SetTrigger("IntoLobbyReadyError");
                return;
            }

            if (!SlotPlayerInformation.Have2TeamsEqual(ServerInformation.Singleton.PlayerList))
            {
                // Pop up a TEAM error box indicating that can only start when all players are ready
                lobbyAnimator.SetTrigger("IntoLobbyTeamError");
                return;
            }
        }

        MyPlayerInformation.SwitchReadyState();
        OnSlotReadyOrStartPress?.Invoke(MyPlayerInformation.SlotIndex, MyPlayerInformation.ReadyState);

        Client.Singleton.SendToServer(new NetReady(MyPlayerInformation.Id));
    }

    public void OnSwitchTeamBtn()
    {
        SlotPlayerInformation MyPlayerInformation = ClientInformation.Singleton.MyPlayerInformation;

        MyPlayerInformation.SwitchTeam();
        SetSwitchTeamBtnColor(MyPlayerInformation.Team);
        OnSlotSwitchTeam?.Invoke(MyPlayerInformation.SlotIndex);

        Client.Singleton.SendToServer(new NetSwitchTeam(MyPlayerInformation.Id));
    }

    public void OnLeaveBtn()
    {
        //backend
        if (ClientInformation.Singleton.IsHost)
            Server.Singleton.Shutdown();
        else
            Client.Singleton.Shutdown();

        //frontend
        OnLobbyLeft?.Invoke();
        OnAllSlotReset?.Invoke();
    }

    public void OnConfirmBtn()
    {
        lobbyAnimator.SetTrigger("IntoLobbyIdle");
    }

    private void OnDeclareHost(bool isHost)
    {
        TMP_Text readyOrStartBtnName = readyOrStartBtn.GetComponentInChildren<TMP_Text>();
        readyOrStartBtnName.SetText(isHost ? "StartGame" : "Ready");
    }

    private void OnDisconnectedClient(SlotPlayerInformation disconnectedPlayer)
    {
        OnPlayerExitedSlot?.Invoke(disconnectedPlayer);
    }

    private void OnNewJoinedPlayer(SlotPlayerInformation joinedPlayer)
    {
        OnPlayerJoinedSlot?.Invoke(joinedPlayer);

        if (joinedPlayer.Id == ClientInformation.Singleton.MyPlayerInformation.Id)
        {
            SetSwitchTeamBtnColor(joinedPlayer.Team);
        }
    }

    private void SetSwitchTeamBtnColor(Team team)
    {
        GameInformation gameInformation = GameInformation.Singleton;
        switchTeamBtn.color = team == Team.Blue ? gameInformation.BlueTeamColor : gameInformation.RedTeamColor;
    }
}