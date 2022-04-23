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
    private SortedSet<byte> blueSlotSet;
    private SortedSet<byte> redSlotSet;

    #region ClassInitMethods
    public ServerInformation()
    {
        if (Singleton == null)
            Singleton = this;

        this.playerList = new List<Player>();
        this.blueSlotSet = new SortedSet<byte>();
        this.redSlotSet = new SortedSet<byte>();

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
            Server.Singleton.OnClientDisconnected += OnClientDisconnected;
            Server.Singleton.OnServerDisconnect += OnServerDisconnect;
        }
        else
        {
            NetUtility.S_SEND_NAME -= this.OnServerReceivedSendNameMessage;
            Server.Singleton.OnClientDisconnected -= OnClientDisconnected;
            Server.Singleton.OnServerDisconnect -= OnServerDisconnect;
        }
    }

    private void OnServerDisconnect()
    {
        this.ResetServerInformation();
    }

    private void OnClientDisconnected(byte disconnectedClientId)
    {
        Player disconnectedPlayer = Player.FindPlayerWithID(ref this.playerList, disconnectedClientId);

        this.playerList.Remove(disconnectedPlayer);
        if (disconnectedPlayer.Team == Team.Blue)
        {
            this.blueSlotSet.Add(disconnectedPlayer.SlotIndex);
        }
        else
        {
            this.redSlotSet.Add(disconnectedPlayer.SlotIndex);
        }
    }

    private void ResetServerInformation()
    {
        this.playerList?.Clear();

        this.blueSlotSet?.Clear();
        this.redSlotSet?.Clear();

        for (byte i = 0; i < (byte)GameInformation.Singleton.MaxPlayer / 2; i++)
        {
            this.blueSlotSet?.Add(i);
            this.redSlotSet?.Add(i);
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
        Team team = GetTeamForNewPlayer();
        byte lobbyIndex = this.GetSlotIndexForNewPlayer(team);
        return new Player(id, team, lobbyIndex, playerName);
    }

    /*
        Choose team which has more slots, if equals then Team.Blue
    */
    private Team GetTeamForNewPlayer()
    {
        return this.blueSlotSet.Count >= this.redSlotSet.Count ? Team.Blue : Team.Red;
    }

    private byte GetSlotIndexForNewPlayer(Team team)
    {
        byte slotIndex = team == Team.Blue ? this.blueSlotSet.Min : this.redSlotSet.Min;
        if (team == Team.Blue)
        {
            this.blueSlotSet.Remove(slotIndex);
        }
        else
        {
            this.redSlotSet.Remove(slotIndex);
        }

        return slotIndex;
    }
}
