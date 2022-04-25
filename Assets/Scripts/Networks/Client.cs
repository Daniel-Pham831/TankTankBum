using System;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Singleton { get; private set; }

    private string playerName;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;
    private bool isClientShutDown = false;

    public Action OnClientDisconnect;
    public Action OnServerDisconnect;

    // Methods
    public void Init(string ip, ushort port, string playerName)
    {
        if (this.isActive)
        {
            this.ClientReset();
        }

        this.driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port);

        this.connection = this.driver.Connect(endPoint);

        Debug.Log($"Attemping to connect to Server on {endPoint.Address}");

        this.isActive = true;
        this.isClientShutDown = false;

        this.RegisterToEvent();

        this.playerName = playerName != "" ? playerName : "I forgot to name myself";
    }

    private void ClientReset()
    {
        this.driver.Dispose();
        this.connection = default(NetworkConnection);
        this.isActive = false;
        this.UnregisterToEvent();
        this.isClientShutDown = true;
    }

    public void Shutdown()
    {
        if (this.isActive)
        {
            this.connection.Disconnect(this.driver);
            this.OnClientDisconnect?.Invoke();
        }
    }
    // public void OnDestroy()
    // {
    //     this.Shutdown();
    // }


    public void Update()
    {
        if (!this.isActive) return;

        this.driver.ScheduleUpdate().Complete();
        this.CheckAlive();
        this.UpdateMessagePump();

        if (this.isClientShutDown)
            this.ClientReset();
    }

    private void CheckAlive()
    {
        if (!this.connection.IsCreated && this.isActive)
        {
            Debug.Log("Something went wrong, lost connection to server!");
            this.Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;
        NetworkEvent.Type cmd;

        while ((cmd = this.connection.PopEvent(this.driver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    this.SendToServer(new NetSendName(this.playerName));
                    break;

                case NetworkEvent.Type.Data:
                    NetUtility.OnData(ref streamReader, default(NetworkConnection));
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Server has been shutdown!!");
                    this.OnServerDisconnect?.Invoke();
                    this.Shutdown();
                    break;
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        this.driver.BeginSend(this.connection, out writer);
        msg.Serialize(ref writer);
        this.driver.EndSend(writer);
    }

    #region Network Received
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += this.OnClientReceivedKeepAliveMessage;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= this.OnClientReceivedKeepAliveMessage;
    }

    private void OnClientReceivedKeepAliveMessage(NetMessage keepAliveMessage)
    {
        // Send it back, to keep both side alive
        this.SendToServer(keepAliveMessage);
    }
    #endregion

}
