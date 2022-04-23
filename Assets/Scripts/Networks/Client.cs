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

    public Action OnServerShutDown;

    // Methods
    public void Init(string ip, ushort port, string playerName)
    {
        this.driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port);

        this.connection = this.driver.Connect(endPoint);

        Debug.Log($"Attemping to connect to Server on {endPoint.Address}");

        this.isActive = true;

        this.RegisterToEvent();

        this.playerName = playerName != "" ? playerName : "I forgot to name myself";
    }

    public void Shutdown()
    {
        if (this.isActive)
        {
            this.SendToServer(new NetDisconnect(PlayerInformation.Singleton.MyPlayerInformation.Id));
            // this.driver.Disconnect(this.connection);

            //this.UnregisterToEvent();
            // this.driver.Dispose();
            this.isActive = false;
            //  connection = default(NetworkConnection);
        }
    }
    private void Disconnect()
    {
        if (this.isActive)
        {
            this.UnregisterToEvent();
            this.driver.Dispose();
            this.isActive = false;
            connection = default(NetworkConnection);
        }
    }

    public void OnDestroy()
    {
        this.Shutdown();
    }

    public void Update()
    {
        if (!this.isActive) return;

        this.driver.ScheduleUpdate().Complete();
        this.CheckAlive();
        this.UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!this.connection.IsCreated && this.isActive)
        {
            Debug.Log("Something went wrong, lost connection to server!");
            this.OnServerShutDown?.Invoke();
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
                    Debug.Log("Server has been Shutdown");
                    this.connection = default(NetworkConnection);
                    this.OnServerShutDown?.Invoke();
                    this.Disconnect();
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
        NetUtility.C_KEEP_ALIVE += this.OnKeepAlive;
        PlayerInformation.Singleton.OnSelfDisconnect += OnSelfDisconnect;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= this.OnKeepAlive;
        PlayerInformation.Singleton.OnSelfDisconnect -= OnSelfDisconnect;
    }

    private void OnSelfDisconnect()
    {
        this.OnServerShutDown?.Invoke();
        this.Disconnect();

    }

    private void OnKeepAlive(NetMessage keepAliveMessage)
    {
        // Send it back, to keep both side alive
        this.SendToServer(keepAliveMessage);
        Debug.Log(this.connection.InternalId);
    }
    #endregion

}
