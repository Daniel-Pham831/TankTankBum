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

    #region ClassInitMethods
    public ServerInformation()
    {
        if (Singleton == null)
            Singleton = this;

        this.playerList = new List<Player>();
        this.idList = new Queue<byte>();
        this.GenerateIdList();
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

    private void GenerateIdList()
    {
        for (byte i = 0; i < (byte)GameInformation.Singleton.MaxPlayer; i++)
        {
            this.idList.Enqueue(i);
        }
    }
    #endregion


    //Server
    private void OnSendNameServer(NetMessage message, NetworkConnection connectedClient)
    {
        NetSendName sendNameMessage = message as NetSendName;
        /*  At this moment server just received a sendNameMessage from connectedClient which contains clientName
            Server need to send a welcomeMessage back to him which contains
                id (server need to assign an id for him)
                number of players in lobby
                a List<Player>
        */

        byte newId = this.GetIdForNewPlayer();
        this.playerList.Add(new Player(newId, sendNameMessage.Name));

        NetWelcome welcomeMessage = new NetWelcome(newId, (byte)this.playerList.Count, this.playerList);
        Server.Singleton.SendToClient(connectedClient, welcomeMessage);
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
