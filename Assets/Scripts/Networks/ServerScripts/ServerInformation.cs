using System;
using System.Collections.Generic;
using Unity.Networking.Transport;

/*
    This class is for storing server information
    playerList -> all of the players in server include the host
    blueSlots -> current available blue slots
    redSlots -> current available red slots
*/
public class ServerInformation
{
    public static ServerInformation Singleton;

    public List<SlotPlayerInformation> PlayerList;
    private SortedSet<byte> availableSlots;
    private List<ReadyState> readySlots;

    #region ClassInitMethods
    public ServerInformation()
    {
        if (Singleton == null)
            Singleton = this;

        PlayerList = new List<SlotPlayerInformation>();
        availableSlots = new SortedSet<byte>();
        readySlots = new List<ReadyState>();

        ResetServerInformation();
        registerToEvent(true);
    }

    ~ServerInformation()
    {
        Singleton = null;
        registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_SEND_NAME += OnServerReceivedSendNameMessage;
            NetUtility.S_SWITCHTEAM += OnServerReceivedSwitchTeamMessage;
            NetUtility.S_READY += OnServerReceivedReadyMessage;

            Server.Singleton.OnClientDisconnected += OnClientDisconnected;
            Server.Singleton.OnServerDisconnect += OnServerDisconnect;
        }
        else
        {
            NetUtility.S_SEND_NAME -= OnServerReceivedSendNameMessage;
            NetUtility.S_SWITCHTEAM -= OnServerReceivedSwitchTeamMessage;
            NetUtility.S_READY -= OnServerReceivedReadyMessage;

            Server.Singleton.OnClientDisconnected -= OnClientDisconnected;
            Server.Singleton.OnServerDisconnect -= OnServerDisconnect;
        }
    }

    private void OnServerReceivedReadyMessage(NetMessage message, NetworkConnection sentClient)
    {
        NetReady readyMessage = message as NetReady;

        SlotPlayerInformation sentPlayer = SlotPlayerInformation.FindSlotPlayerWithIDAndRemove(ref PlayerList, readyMessage.Id);

        if (sentPlayer.IsHost)
        {
            Server.Singleton.BroadCast(new NetStartGame());
            return;
        }

        //Switch ReadyState
        if (sentPlayer != null)
        {
            sentPlayer.SwitchReadyState();
            PlayerList.Add(sentPlayer);
        }

        Server.Singleton.BroadCastExcept(readyMessage, sentClient);
    }

    private void OnServerReceivedSwitchTeamMessage(NetMessage message, NetworkConnection sentClient)
    {
        NetSwitchTeam switchTeamMessage = message as NetSwitchTeam;

        SlotPlayerInformation sentPlayer = SlotPlayerInformation.FindSlotPlayerWithIDAndRemove(ref PlayerList, switchTeamMessage.Id);

        //SwitchTeam
        if (sentPlayer != null)
        {
            sentPlayer.SwitchTeam();
            PlayerList.Add(sentPlayer);
        }

        Server.Singleton.BroadCastExcept(switchTeamMessage, sentClient);
    }

    private void OnServerDisconnect()
    {
        ResetServerInformation();
    }

    private void OnClientDisconnected(byte disconnectedClientId)
    {
        SlotPlayerInformation disconnectedPlayer = SlotPlayerInformation.FindSlotPlayerWithID(PlayerList, disconnectedClientId);
        PlayerList.Remove(disconnectedPlayer);

        availableSlots.Add(disconnectedPlayer.SlotIndex);
    }

    private void ResetServerInformation()
    {
        PlayerList?.Clear();
        availableSlots?.Clear();

        for (byte i = 0; i < (byte)GameInformation.Singleton.MaxPlayer; i++)
        {
            availableSlots.Add(i);
            readySlots.Add(ReadyState.Unready);
        }
    }

    #endregion

    //Server
    private void OnServerReceivedSendNameMessage(NetMessage message, NetworkConnection connectedClient)
    {
        /*  At this moment server just received a sendNameMessage from connectedClient which contains his name
            Server need to send a welcomeMessage back to him which contains
                id + team + lobbyIndex(server need to assign an id for him)
                number of players in lobby
                a List<Player>
        */
        NetSendName sendNameMessage = message as NetSendName;
        SlotPlayerInformation connectedPlayer = GetNewPlayerInformation((byte)connectedClient.InternalId, sendNameMessage.Name);
        Server.Singleton.SendToClient(connectedClient, new NetWelcome(connectedPlayer, (byte)PlayerList.Count, PlayerList));

        // BroadCast to all player that a new player just joined
        Server.Singleton.BroadCastExcept(new NetJoin(connectedPlayer), connectedClient);

        PlayerList.Add(connectedPlayer);
    }

    private SlotPlayerInformation GetNewPlayerInformation(byte id, string playerName)
    {
        Team team = Team.Blue; //new joined player will be on Team.Blue by default
        byte lobbyIndex = GetSlotIndexForNewPlayer();
        return new SlotPlayerInformation(id, team, lobbyIndex, playerName, ReadyState.Unready);
    }

    private byte GetSlotIndexForNewPlayer()
    {
        byte slotIndex = availableSlots.Min;
        availableSlots.Remove(slotIndex);
        return slotIndex;
    }
}
