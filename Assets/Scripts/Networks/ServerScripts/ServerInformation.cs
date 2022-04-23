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
    private Queue<byte> blueSlots;
    private Queue<byte> redSlots;

    #region ClassInitMethods
    public ServerInformation()
    {
        if (Singleton != null) return;

        if (Singleton == null)
            Singleton = this;

        this.playerList = new List<Player>();
        this.blueSlots = new Queue<byte>();
        this.redSlots = new Queue<byte>();

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
            NetUtility.S_SEND_NAME += this.OnSendNameServer;
            NetUtility.S_DISCONNECT += this.OnDisconnectedClientOnServer;
            MainMenuUI.Singleton.OnLobbyLeft += this.ResetServerInformation;
        }
        else
        {
            NetUtility.S_SEND_NAME -= this.OnSendNameServer;
            NetUtility.S_DISCONNECT -= this.OnDisconnectedClientOnServer;
            MainMenuUI.Singleton.OnLobbyLeft -= this.ResetServerInformation;
        }
    }

    private void OnDisconnectedClientOnServer(NetMessage message, NetworkConnection disconnectedClient)
    {
        NetDisconnect disconnectedMessage = message as NetDisconnect;
        Server.Singleton.BroadCast(disconnectedMessage);

        Player disconnectedPlayer = Player.FindPlayer(ref this.playerList, disconnectedMessage.DisconnectedClientId);

        this.playerList.Remove(disconnectedPlayer);
        if (disconnectedPlayer.Team == Team.Blue)
            this.blueSlots.Enqueue(disconnectedPlayer.SlotIndex);
        else
            this.redSlots.Enqueue(disconnectedPlayer.SlotIndex);
    }

    public void ResetServerInformation()
    {
        this.playerList.Clear();
        this.blueSlots.Clear();
        this.redSlots.Clear();
        for (byte i = 0; i < (byte)GameInformation.Singleton.MaxPlayer / 2; i++)
        {
            this.blueSlots.Enqueue(i);
            this.redSlots.Enqueue(i);
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
        Player connectedPlayer = this.GetNewPlayerInformation((byte)connectedClient.InternalId, sendNameMessage.Name);
        Server.Singleton.SendToClient(connectedClient, new NetWelcome(connectedPlayer, (byte)this.playerList.Count, this.playerList));

        // BroadCast to all player that a new player just joined
        Server.Singleton.BroadCastExcept(new NetJoin(connectedPlayer), connectedClient);

        this.playerList.Add(connectedPlayer);
    }

    private Player GetNewPlayerInformation(byte id, string playerName)
    {
        Team team = this.playerList.Count % 2 == 0 ? Team.Blue : Team.Red;
        byte lobbyIndex = this.GetSlotIndexForNewPlayer(team);
        return new Player(id, team, lobbyIndex, playerName);
    }

    private byte GetSlotIndexForNewPlayer(Team team)
    {
        return team == Team.Blue ? this.blueSlots.Dequeue() : this.redSlots.Dequeue();
    }
}
