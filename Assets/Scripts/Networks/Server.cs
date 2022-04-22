using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Server Singleton { get; private set; }
    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private float keepAliveTickRate = 20f;
    private float lastKeepAlive;

    public Action connectionDropped;

    // Methods
    public void Init(ushort port, int maximumConnection)
    {
        this.driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = port;

        if (this.driver.Bind(endPoint) != 0)
        {
            Debug.Log($"Unable to bind to port {endPoint.Port}");
            return;
        }
        else
        {
            this.driver.Listen();
            Debug.Log($"Currently listening on port {endPoint.Port}");
        }

        this.connections = new NativeList<NetworkConnection>(maximumConnection, Allocator.Persistent);
        this.isActive = true;

        //Init server information
        ServerInformation serverInfomation = new ServerInformation();
    }

    public void Shutdown()
    {
        if (this.isActive)
        {
            this.driver.Dispose();
            this.connections.Dispose();
            this.isActive = false;
        }
    }

    public void OnDestroy()
    {
        this.Shutdown();
    }

    public void Update()
    {
        if (!this.isActive) return;

        this.KeepAlive();

        this.driver.ScheduleUpdate().Complete();

        this.CleanupConnections();
        this.AcceptNewConnections();
        this.UpdateMessagePump();
    }

    private void KeepAlive()
    {
        if (Time.time - this.lastKeepAlive > this.keepAliveTickRate)
        {
            this.lastKeepAlive = Time.time;
            this.BroadCast(new NetKeepAlive());
        }
    }

    private void CleanupConnections()
    {
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (!this.connections[i].IsCreated)
            {
                this.connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = this.driver.Accept()) != default(NetworkConnection))
        {
            this.connections.Add(c);
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;
        for (int i = 0; i < this.connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = this.driver.PopEventForConnection(this.connections[i], out streamReader)) != NetworkEvent.Type.Empty)
            {
                switch (cmd)
                {
                    case NetworkEvent.Type.Data:
                        NetUtility.OnData(ref streamReader, this.connections[i], this);
                        break;

                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("Client disconnected from the server");
                        this.connections[i] = default(NetworkConnection);
                        this.connections.RemoveAt(i);
                        this.connectionDropped?.Invoke();

                        break;
                }
            }
        }
    }

    // Server specific
    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        this.driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        this.driver.EndSend(writer);
    }

    public void BroadCast(NetMessage msg)
    {
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (this.connections[i].IsCreated)
            {
                Debug.Log($"Sending {msg.Code} to: {this.connections[i].InternalId}");
                this.SendToClient(this.connections[i], msg);
            }
        }
    }

    public void BroadCastExcept(NetMessage msg, NetworkConnection exceptClient)
    {
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (this.connections[i].IsCreated && this.connections[i] != exceptClient)
            {
                Debug.Log($"Sending {msg.Code} to: {this.connections[i].InternalId}");
                this.SendToClient(this.connections[i], msg);
            }
        }
    }
}
