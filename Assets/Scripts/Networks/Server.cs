using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    private readonly float keepAliveTickRate = 20f;
    private NetworkDriver driver { get; set; }
    private NativeList<NetworkConnection> connections;
    private bool isActive;
    private bool isServerShutDown;
    private float lastKeepAlive;

    public static Server Singleton { get; private set; }

    public Action<byte> OnClientDisconnected { get; set; }

    public Action OnServerDisconnect { get; set; }


    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        registerToEvent(true);
    }

    private void registerToEvent(bool confirm)
    {
        if (confirm)
        {
            NetUtility.S_PING += OnServerReceivedPingMessage;
        }
        else
        {
            NetUtility.S_PING -= OnServerReceivedPingMessage;
        }
    }

    private void OnServerReceivedPingMessage(NetMessage message, NetworkConnection sentClient)
    {
        SendToClient(sentClient, message);
    }

    // Methods
    public void Init(ushort port, int maximumConnection)
    {
        if (isActive)
        {
            ServerReset();
        }

        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = port;

        if (driver.Bind(endPoint) != 0)
        {
            Debug.Log($"Unable to bind to port {endPoint.Port}");
            return;
        }
        else
        {
            driver.Listen();
            Debug.Log($"Currently listening on port {endPoint.Port}");
        }

        connections = new NativeList<NetworkConnection>(maximumConnection, Allocator.Persistent);
        isActive = true;
        isServerShutDown = false;

        // Init server information
        if (ServerInformation.Singleton == null)
        {
            _ = new ServerInformation();
        }
    }

    private void OnDestroy()
    {
        if (isActive)
            ServerReset();
    }

    private void ServerReset()
    {
        driver.Dispose();
        connections.Dispose();
        isActive = false;
        isServerShutDown = false;
    }

    public void Shutdown()
    {
        if (isActive)
        {
            foreach (NetworkConnection connection in connections)
            {
                driver.Disconnect(connection);
            }

            OnServerDisconnect?.Invoke();
            isServerShutDown = true;
        }
    }

    public void Update()
    {
        if (!isActive)
        {
            return;
        }

        KeepAlive();

        driver.ScheduleUpdate().Complete();

        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();

        if (isServerShutDown)
        {
            ServerReset();
        }
    }

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > keepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            BroadCast(new NetKeepAlive());
        }
    }

    private void CleanupConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default)
        {
            connections.Add(c);
        }
    }

    private void UpdateMessagePump()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out DataStreamReader streamReader)) != NetworkEvent.Type.Empty)
            {
                switch (cmd)
                {
                    case NetworkEvent.Type.Data:
                        NetUtility.OnData(ref streamReader, connections[i], this);
                        break;

                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("Client disconnected from the server");
                        BroadCast(new NetDisconnect((byte)connections[i].InternalId));
                        OnClientDisconnected?.Invoke((byte)connections[i].InternalId);

                        connections[i] = default;
                        break;
                }
            }
        }
    }

    // Server specific
    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        driver.BeginSend(connection, out DataStreamWriter writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void BroadCast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                SendToClient(connections[i], msg);
            }
        }
    }

    public void BroadCastExcept(NetMessage msg, NetworkConnection exceptClient)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated && connections[i] != exceptClient)
            {
                SendToClient(connections[i], msg);
            }
        }
    }
}
