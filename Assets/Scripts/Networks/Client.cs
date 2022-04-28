using System;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    private NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;
    private bool isClientShutDown = false;
    private string playerName;

    public static Client Singleton { get; private set; }

    public Action OnClientDisconnect;
    public Action OnServerDisconnect;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    // Methods
    public void Init(string ip, ushort port, string playerName)
    {
        if (isActive)
        {
            ClientReset();
        }

        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port);

        connection = driver.Connect(endPoint);

        Debug.Log($"Attemping to connect to Server on {endPoint.Address}");

        isActive = true;
        isClientShutDown = false;

        RegisterToEvent();

        playerName = playerName != "" ? playerName : "I forgot to name myself";
    }

    private void ClientReset()
    {
        driver.Dispose();
        connection = default(NetworkConnection);
        isActive = false;
        UnregisterToEvent();
        isClientShutDown = true;
    }

    public void Shutdown()
    {
        if (isActive)
        {
            connection.Disconnect(driver);
            OnClientDisconnect?.Invoke();
        }
    }
    // public void OnDestroy()
    // {
    //     Shutdown();
    // }


    public void Update()
    {
        if (!isActive) return;

        driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();

        if (isClientShutDown)
            ClientReset();
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            Debug.Log("Something went wrong, lost connection to server!");
            Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;
        NetworkEvent.Type cmd;

        while ((cmd = connection.PopEvent(driver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    SendToServer(new NetSendName(playerName));
                    break;

                case NetworkEvent.Type.Data:
                    NetUtility.OnData(ref streamReader, default(NetworkConnection));
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Server has been shutdown!!");
                    OnServerDisconnect?.Invoke();
                    Shutdown();
                    break;
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    #region Network Received
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnClientReceivedKeepAliveMessage;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnClientReceivedKeepAliveMessage;
    }

    private void OnClientReceivedKeepAliveMessage(NetMessage keepAliveMessage)
    {
        // Send it back, to keep both side alive
        SendToServer(keepAliveMessage);
    }
    #endregion

}
