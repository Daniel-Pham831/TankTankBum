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

    public List<Player> playerList;
    private SortedSet<byte> availableSlotSet;

    #region ClassInitMethods
    public ServerInformation()
    {
        if (Singleton == null)
            Singleton = this;

        this.playerList = new List<Player>();
        this.availableSlotSet = new SortedSet<byte>();

        this.ResetServerInformation();
        this.registerToEvent(true);
    }

    ~ServerInformation()
    {
        Singleton = null;
        this.registerToEvent(false);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_SEND_NAME += this.OnServerReceivedSendNameMessage;
            NetUtility.S_SWITCHTEAM += this.OnServerReceivedSwitchTeamMessage;

            Server.Singleton.OnClientDisconnected += OnClientDisconnected;
            Server.Singleton.OnServerDisconnect += OnServerDisconnect;
        }
        else
        {
            NetUtility.S_SEND_NAME -= this.OnServerReceivedSendNameMessage;
            NetUtility.S_SWITCHTEAM -= this.OnServerReceivedSwitchTeamMessage;

            Server.Singleton.OnClientDisconnected -= OnClientDisconnected;
            Server.Singleton.OnServerDisconnect -= OnServerDisconnect;
        }
    }

    private void OnServerReceivedSwitchTeamMessage(NetMessage message, NetworkConnection sentClient)
    {
        NetSwitchTeam switchTeamMessage = message as NetSwitchTeam;

        Player sentPlayer = Player.FindPlayerWithIDAndRemove(ref this.playerList, switchTeamMessage.Id);

        //SwitchTeam
        if (sentPlayer != null)
        {
            sentPlayer.SwitchTeam();
            this.playerList.Add(sentPlayer);
        }

        Server.Singleton.BroadCastExcept(switchTeamMessage, sentClient);
    }

    private void OnServerDisconnect()
    {
        this.ResetServerInformation();
    }

    private void OnClientDisconnected(byte disconnectedClientId)
    {
        Player disconnectedPlayer = Player.FindPlayerWithID(this.playerList, disconnectedClientId);
        this.playerList.Remove(disconnectedPlayer);

        this.availableSlotSet.Add(disconnectedPlayer.SlotIndex);
    }

    private void ResetServerInformation()
    {
        this.playerList?.Clear();
        this.availableSlotSet?.Clear();

        for (byte i = 0; i < (byte)GameInformation.Singleton.MaxPlayer; i++)
        {
            this.availableSlotSet.Add(i);
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
        Player connectedPlayer = this.GetNewPlayerInformation((byte)connectedClient.InternalId, sendNameMessage.Name);
        Server.Singleton.SendToClient(connectedClient, new NetWelcome(connectedPlayer, (byte)this.playerList.Count, this.playerList));

        // BroadCast to all player that a new player just joined
        Server.Singleton.BroadCastExcept(new NetJoin(connectedPlayer), connectedClient);

        this.playerList.Add(connectedPlayer);
    }

    private Player GetNewPlayerInformation(byte id, string playerName)
    {
        Team team = Team.Blue; //new joined player will be on Team.Blue on default
        byte lobbyIndex = this.GetSlotIndexForNewPlayer();
        return new Player(id, team, lobbyIndex, playerName);
    }

    private byte GetSlotIndexForNewPlayer()
    {
        byte slotIndex = this.availableSlotSet.Min;
        this.availableSlotSet.Remove(slotIndex);
        return slotIndex;
    }
}
