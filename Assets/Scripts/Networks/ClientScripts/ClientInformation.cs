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

    public Player MyPlayerInformation;
    public List<Player> PlayerList;
    public List<byte> IdList;
    public List<string> NameList;
    public bool IsHost;

    public Action<Player> OnNewJoinedPlayer;
    public Action<Player> OnDisconnectedClient;
    public Action<bool> OnDeclareHost;
    public Action<byte> OnPlayerSwitchTeam;
    void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        this.MyPlayerInformation = new Player();
        this.IdList = new List<byte>();
        this.NameList = new List<string>();

        this.ResetClientInformation();

        DontDestroyOnLoad(this.gameObject);
    }

    private void ResetClientInformation()
    {
        this.PlayerList?.Clear();
        this.IdList?.Clear();
        this.NameList?.Clear();
        this.IsHost = false;
    }

    public void SwitchMyTeam()
    {
        this.MyPlayerInformation.Team = this.MyPlayerInformation.Team == Team.Blue ? Team.Red : Team.Blue;
    }

    private void Start()
    {
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
            NetUtility.C_WELCOME += this.OnClientReceivedWelcomeMessage;
            NetUtility.C_JOIN += this.OnClientReceivedJoinMessage;
            NetUtility.C_DISCONNECT += this.OnClientReceivedDisconnectedMessage;
            NetUtility.C_SWITCHTEAM += this.OnClientReceivedSwitchTeamMessage;
            Client.Singleton.OnClientDisconnect += this.OnClientDisconnect;

            MainMenuUI.Singleton.OnHostOrJoinRoom += this.OnHostOrJoinRoom;
        }
        else
        {
            NetUtility.C_WELCOME -= this.OnClientReceivedWelcomeMessage;
            NetUtility.C_JOIN -= this.OnClientReceivedJoinMessage;
            NetUtility.C_DISCONNECT -= this.OnClientReceivedDisconnectedMessage;
            NetUtility.C_SWITCHTEAM -= this.OnClientReceivedSwitchTeamMessage;
            Client.Singleton.OnClientDisconnect -= this.OnClientDisconnect;

            MainMenuUI.Singleton.OnHostOrJoinRoom -= this.OnHostOrJoinRoom;
        }
    }

    private void OnClientReceivedSwitchTeamMessage(NetMessage message)
    {
        NetSwitchTeam switchTeamMessage = message as NetSwitchTeam;

        Player sentPlayer = Player.FindPlayerWithIDAndRemove(ref this.PlayerList, switchTeamMessage.Id);

        //SwitchTeam
        if (sentPlayer != null)
        {
            sentPlayer.SwitchTeam();
            this.PlayerList.Add(sentPlayer);
            this.OnPlayerSwitchTeam?.Invoke(sentPlayer.SlotIndex);
        }
        Debug.Log($"Received Switch Team for player {sentPlayer.Name}");
    }

    private void OnClientDisconnect()
    {
        this.ResetClientInformation();
    }

    private void OnClientReceivedDisconnectedMessage(NetMessage message)
    {
        NetDisconnect disconnectMessage = message as NetDisconnect;

        Player disconnectedPlayer = Player.FindPlayerWithID(this.PlayerList, disconnectMessage.DisconnectedClientId);

        this.OnDisconnectedClient?.Invoke(disconnectedPlayer);

        this.PlayerList.Remove(disconnectedPlayer);
        this.IdList.Remove(disconnectedPlayer.Id);
        this.NameList.Remove(disconnectedPlayer.Name);
    }

    private void OnHostOrJoinRoom(string inputName)
    {
        this.MyPlayerInformation.Name = inputName;
    }

    private void OnClientReceivedJoinMessage(NetMessage message)
    {
        NetJoin joinMessage = message as NetJoin;

        this.PlayerList.Add(joinMessage.JoinedPlayer);

        Debug.Log($"{joinMessage.JoinedPlayer.Name} just joined");

        this.IdList.Add(joinMessage.JoinedPlayer.Id);
        this.NameList.Add(joinMessage.JoinedPlayer.Name);

        this.OnNewJoinedPlayer?.Invoke(joinMessage.JoinedPlayer);
    }

    private void OnClientReceivedWelcomeMessage(NetMessage message)
    {
        NetWelcome welcomeMessage = message as NetWelcome;

        this.MyPlayerInformation = welcomeMessage.MyPlayerInformation;

        this.PlayerList = welcomeMessage.PlayerList;

        foreach (Player player in this.PlayerList)
        {
            this.IdList.Add(player.Id);
            this.NameList.Add(player.Name);
            this.OnNewJoinedPlayer?.Invoke(player);
        }
        Debug.Log($"\nMy ID:{this.MyPlayerInformation.Id} My Name:{this.MyPlayerInformation.Name}");

        if (this.MyPlayerInformation.Id == GameInformation.Singleton.HostId)
        {
            Debug.Log("I'm the host");
            this.IsHost = true;
        }
        else
        {
            Debug.Log("I'm a client");
            this.IsHost = false;
        }

        this.OnDeclareHost?.Invoke(this.IsHost);
        this.OnNewJoinedPlayer?.Invoke(this.MyPlayerInformation);
    }
}
