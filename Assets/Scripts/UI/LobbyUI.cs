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

    public Action<Player> OnPlayerJoinedSlot;
    public Action<Player> OnPlayerExitedSlot;
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
        this.registerToEvent(true);

        this.GenerateAllSlots();
    }

    private void OnDestroy()
    {
        Singleton = null;
        //   this.registerToEvent(false);
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
        this.OnSlotReadyOrStartPress?.Invoke(slotIndex, readyState);
    }

    private void OnPlayerSwitchTeam(byte slotIndex)
    {
        this.OnSlotSwitchTeam?.Invoke(slotIndex);
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

    public void OnStartOrReadyBtn()
    {
        Player MyPlayerInformation = ClientInformation.Singleton.MyPlayerInformation;

        if (MyPlayerInformation.IsHost && !Player.HaveAllPlayersReadied(ClientInformation.Singleton.PlayerList))
        {
            // If the host press start btn
            // Pop up a message box indicating that can only start when all players are ready
            Debug.Log("There are still players who haven't readied yet");
            this.lobbyAnimator.SetTrigger("IntoLobbyPopup");
            return;
        }

        MyPlayerInformation.SwitchReadyState();
        this.OnSlotReadyOrStartPress?.Invoke(MyPlayerInformation.SlotIndex, MyPlayerInformation.ReadyState);

        Client.Singleton.SendToServer(new NetReady(MyPlayerInformation.Id));
    }

    public void OnSwitchTeamBtn()
    {
        Player MyPlayerInformation = ClientInformation.Singleton.MyPlayerInformation;

        MyPlayerInformation.SwitchTeam();
        this.SetSwitchTeamBtnColor(MyPlayerInformation.Team);
        this.OnSlotSwitchTeam?.Invoke(MyPlayerInformation.SlotIndex);

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
        this.OnLobbyLeft?.Invoke();
        this.OnAllSlotReset?.Invoke();
    }

    public void OnConfirmBtn()
    {
        this.lobbyAnimator.SetTrigger("IntoLobbyIdle");
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

        if (joinedPlayer.Id == ClientInformation.Singleton.MyPlayerInformation.Id)
        {
            this.SetSwitchTeamBtnColor(joinedPlayer.Team);
        }
    }

    private void SetSwitchTeamBtnColor(Team team)
    {
        GameInformation gameInformation = GameInformation.Singleton;
        this.switchTeamBtn.color = team == Team.Blue ? gameInformation.BlueTeamColor : gameInformation.RedTeamColor;
    }
}