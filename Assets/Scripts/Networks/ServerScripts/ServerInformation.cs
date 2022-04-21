using System;
using System.Collections.Generic;
using Unity.Networking.Transport;


/*
    This class is for storing players information

    *Need to add a access prevention from client
    *Only Server is allow to access the class
*/
public class ServerInformation
{
    public static ServerInformation Singleton;
    public List<Player> playerList;
    private Queue<byte> idList;
    private Queue<byte> blueSlots;
    private Queue<byte> redSlots;

    #region ClassInitMethods
    public ServerInformation()
    {
        if (Singleton == null)
            Singleton = this;

        this.playerList = new List<Player>();
        this.GenerateIdList();
        this.GenerateLobbySlots();
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
            NetUtility.S_SEND_NAME += this.OnSendNameServer;
        }
        else
        {
            NetUtility.S_SEND_NAME -= this.OnSendNameServer;
        }
    }

    private void GenerateLobbySlots()
    {
        this.blueSlots = new Queue<byte>();
        this.redSlots = new Queue<byte>();
        for (byte i = 0; i < (byte)GameInformation.Singleton.MaxPlayer; i++)
        {
            this.blueSlots.Enqueue(i);
            this.redSlots.Enqueue(i);
        }
    }

    private void GenerateIdList()
    {
        this.idList = new Queue<byte>();
        for (byte i = 0; i < (byte)GameInformation.Singleton.MaxPlayer; i++)
        {
            this.idList.Enqueue(i);
        }
    }
    #endregion


    //Server
    private void OnSendNameServer(NetMessage message, NetworkConnection connectedClient)
    {
        /*  At this moment server just received a sendNameMessage from connectedClient which contains his name
            Server need to send a welcomeMessage back to him which contains
                id + team + lobbyIndex(server need to assign an id for him)
                number of players in lobby
                a List<Player>
        */
        NetSendName sendNameMessage = message as NetSendName;
        Player connectedPlayer = this.GetNewPlayerInformation(sendNameMessage.Name);
        Server.Singleton.SendToClient(connectedClient, new NetWelcome(connectedPlayer, (byte)this.playerList.Count, this.playerList));

        // BroadCast to all player that a new player just joined
        Server.Singleton.BroadCastExcept(new NetJoin(connectedPlayer), connectedClient);

        this.playerList.Add(connectedPlayer);
    }

    private Player GetNewPlayerInformation(string playerName)
    {
        byte id = GetIdForNewPlayer();
        Team team = id % 2 == 0 ? Team.Blue : Team.Red;
        byte lobbyIndex = this.GetSlotIndexForNewPlayer(team);
        return new Player(id, team, lobbyIndex, playerName);
    }

    private byte GetSlotIndexForNewPlayer(Team team)
    {
        return team == Team.Blue ? this.blueSlots.Dequeue() : this.redSlots.Dequeue();
    }

    private byte GetIdForNewPlayer()
    {
        return this.idList.Dequeue();
    }

    private void ReturnToIdList(byte id)
    {
        this.idList.Enqueue(id);
    }
}
