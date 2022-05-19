using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is for storing player information
    PlayerList -> all of the players in server include the host exclude MyPlayerInformation
    MyPlayerInformation -> this client info
    IdList -> id list of all players exclude MyPlayerInformation.Id
    NameList -> name list of all players excluce MyPlayerInformation.Name
    IsHost -> Is this client a Host ?
*/
public class ClientInformation : MonoBehaviour
{
    public static ClientInformation Singleton { get; private set; }

    public SlotPlayerInformation MyPlayerInformation;
    public List<SlotPlayerInformation> PlayerList;
    public List<byte> IdList;
    public List<string> NameList;
    public bool IsHost;

    public Action<SlotPlayerInformation> OnNewJoinedPlayer;
    public Action<SlotPlayerInformation> OnDisconnectedClient;
    public Action<bool> OnDeclareHost;
    public Action<byte> OnPlayerSwitchTeam;
    public Action<byte, ReadyState> OnPlayerSwitchReadyState;

    public Action StartGame;
    void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        MyPlayerInformation = new SlotPlayerInformation();
        IdList = new List<byte>();
        NameList = new List<string>();

        ResetClientInformation();

        DontDestroyOnLoad(gameObject);
    }

    private void ResetClientInformation()
    {
        PlayerList?.Clear();
        IdList?.Clear();
        NameList?.Clear();
        IsHost = false;
    }

    private void Start()
    {
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.C_WELCOME += OnClientReceivedWelcomeMessage;
            NetUtility.C_JOIN += OnClientReceivedJoinMessage;
            NetUtility.C_DISCONNECT += OnClientReceivedDisconnectedMessage;
            NetUtility.C_SWITCHTEAM += OnClientReceivedSwitchTeamMessage;
            NetUtility.C_READY += OnClientReceivedReadyMessage;
            NetUtility.C_START += OnClientReceivedStartGameMessage;

            Client.Singleton.OnClientDisconnect += OnClientDisconnect;

            MainMenuUI.Singleton.OnHostOrJoinRoom += OnHostOrJoinRoom;
        }
        else
        {
            NetUtility.C_WELCOME -= OnClientReceivedWelcomeMessage;
            NetUtility.C_JOIN -= OnClientReceivedJoinMessage;
            NetUtility.C_DISCONNECT -= OnClientReceivedDisconnectedMessage;
            NetUtility.C_SWITCHTEAM -= OnClientReceivedSwitchTeamMessage;
            NetUtility.C_READY -= OnClientReceivedReadyMessage;
            NetUtility.C_START -= OnClientReceivedStartGameMessage;

            Client.Singleton.OnClientDisconnect -= OnClientDisconnect;

            MainMenuUI.Singleton.OnHostOrJoinRoom -= OnHostOrJoinRoom;
        }
    }

    private void OnClientReceivedStartGameMessage(NetMessage message)
    {
        StartGame?.Invoke();
        Debug.Log("START GAME");
    }

    private void OnClientReceivedReadyMessage(NetMessage message)
    {
        NetReady readyMessage = message as NetReady;

        SlotPlayerInformation sentPlayer = SlotPlayerInformation.FindSlotPlayerWithIDAndRemove(ref PlayerList, readyMessage.Id);

        //Switch ReadyState
        if (sentPlayer != null)
        {
            sentPlayer.SwitchReadyState();
            PlayerList.Add(sentPlayer);
            OnPlayerSwitchReadyState?.Invoke(sentPlayer.SlotIndex, sentPlayer.ReadyState);
        }
    }

    private void OnClientReceivedSwitchTeamMessage(NetMessage message)
    {
        NetSwitchTeam switchTeamMessage = message as NetSwitchTeam;

        SlotPlayerInformation sentPlayer = SlotPlayerInformation.FindSlotPlayerWithIDAndRemove(ref PlayerList, switchTeamMessage.Id);

        //SwitchTeam
        if (sentPlayer != null)
        {
            sentPlayer.SwitchTeam();
            PlayerList.Add(sentPlayer);
            OnPlayerSwitchTeam?.Invoke(sentPlayer.SlotIndex);
        }
    }

    private void OnClientDisconnect()
    {
        ResetClientInformation();
    }

    private void OnClientReceivedDisconnectedMessage(NetMessage message)
    {
        NetDisconnect disconnectMessage = message as NetDisconnect;

        SlotPlayerInformation disconnectedPlayer = SlotPlayerInformation.FindSlotPlayerWithID(PlayerList, disconnectMessage.DisconnectedClientId);

        OnDisconnectedClient?.Invoke(disconnectedPlayer);

        PlayerList.Remove(disconnectedPlayer);
        IdList.Remove(disconnectedPlayer.Id);
        NameList.Remove(disconnectedPlayer.Name);
    }

    private void OnHostOrJoinRoom(string inputName)
    {
        MyPlayerInformation.Name = inputName;
    }

    private void OnClientReceivedJoinMessage(NetMessage message)
    {
        NetJoin joinMessage = message as NetJoin;

        PlayerList.Add(joinMessage.JoinedPlayer);

        Debug.Log($"{joinMessage.JoinedPlayer.Name} just joined");

        IdList.Add(joinMessage.JoinedPlayer.Id);
        NameList.Add(joinMessage.JoinedPlayer.Name);

        OnNewJoinedPlayer?.Invoke(joinMessage.JoinedPlayer);
    }

    private void OnClientReceivedWelcomeMessage(NetMessage message)
    {
        NetWelcome welcomeMessage = message as NetWelcome;

        MyPlayerInformation = welcomeMessage.MyPlayerInformation;

        PlayerList = welcomeMessage.PlayerList;

        foreach (SlotPlayerInformation player in PlayerList)
        {
            IdList.Add(player.Id);
            NameList.Add(player.Name);
            OnNewJoinedPlayer?.Invoke(player);
        }

        Debug.Log($"\nMy ID:{MyPlayerInformation.Id} My Name:{MyPlayerInformation.Name}");

        if (MyPlayerInformation.Id == GameInformation.Singleton.HostId)
        {
            Debug.Log("I'm the host");
            IsHost = true;
        }
        else
        {
            Debug.Log("I'm a client");
            IsHost = false;
        }

        OnDeclareHost?.Invoke(IsHost);
        OnNewJoinedPlayer?.Invoke(MyPlayerInformation);
    }
}
