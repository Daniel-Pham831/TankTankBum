using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    private readonly float keepAliveTickRate = 20f;

    public static Server Singleton { get; private set; }

    public Action<byte> OnClientDisconnected { get; set; }

    public Action OnServerDisconnect { get; set; }

    public NetworkDriver Driver { get; set; }

    private NativeList<NetworkConnection> connections;
    private bool isActive;
    private bool isServerShutDown;
    private float lastKeepAlive;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
    }

    // Methods
    public void Init(ushort port, int maximumConnection)
    {
        if (isActive)
        {
            ServerReset();
        }

        Driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = port;

        if (Driver.Bind(endPoint) != 0)
        {
            Debug.Log($"Unable to bind to port {endPoint.Port}");
            return;
        }
        else
        {
            Driver.Listen();
            Debug.Log($"Currently listening on port {endPoint.Port}");
        }

        connections = new NativeList<NetworkConnection>(maximumConnection, Allocator.Persistent);
        isActive = true;
        isServerShutDown = false;

        // Init server information
        if (ServerInformation.Singleton == null)
        {
            ServerInformation serverInfomation = new();
        }
    }

    private void ServerReset()
    {
        Driver.Dispose();
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
                Driver.Disconnect(connection);
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

        Driver.ScheduleUpdate().Complete();

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
        while ((c = Driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
        }
    }

    private void UpdateMessagePump()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = Driver.PopEventForConnection(connections[i], out DataStreamReader streamReader)) != NetworkEvent.Type.Empty)
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

                        connections[i] = default(NetworkConnection);
                        break;
                }
            }
        }
    }

    // Server specific
    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        Driver.BeginSend(connection, out DataStreamWriter writer);
        msg.Serialize(ref writer);
        Driver.EndSend(writer);
    }

    public void BroadCast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                Debug.Log($"Sending {msg.Code} to: {connections[i].InternalId}");
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
                Debug.Log($"Sending {msg.Code} to: {connections[i].InternalId}");
                SendToClient(connections[i], msg);
            }
        }
    }
}
