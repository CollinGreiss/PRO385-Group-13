using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

// Main networking manager - attach this to a GameObject in your scene
public class NetworkManager : MonoBehaviour
{

    public static NetworkManager Instance { get; private set; }

     void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [Header("Network Settings")]
    public int port = 7777;
    public int discoveryPort = 7778;

    private LANHost host;
    private LANClient client;
    private bool isHost = false;
    private bool isClient = false;

    void Start()
    {
    }

    public void StartHost()
    {
        if (isHost || isClient) return;

        host = new LANHost(port, discoveryPort);
        host.OnClientConnected += OnClientConnected;
        host.OnClientDisconnected += OnClientDisconnected;
        host.OnCommandReceived += OnCommandReceived;
        host.StartHost();

        isHost = true;
        
        Debug.Log("Host started");
        
    }

    public void StartClient()
    {

        if (isHost || isClient) return;

        client = new LANClient(port, discoveryPort);
        client.OnConnectedToHost += OnConnectedToHost;
        client.OnDisconnectedFromHost += OnDisconnectedFromHost;
        client.OnCommandReceived += OnCommandReceived;
        client.StartClient();

        isClient = true;

        Debug.Log("Client started, searching for host");

    }

    void SendCommand(string command)
    {

        if (string.IsNullOrEmpty(command)) return;

        if (isHost && host != null)
        {
            host.SendCommandToClients(command);
            Debug.Log($"Host sent: {command}");
        }
        else if (isClient && client != null)
        {
            client.SendCommandToHost(command);
            Debug.Log($"Client sent: {command}");
        }
    }

    // Event handlers
    void OnClientConnected(string clientId)
    {
        Debug.Log($"Client connected: {clientId}");
    }

    void OnClientDisconnected(string clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
    }

    void OnConnectedToHost()
    {
        Debug.Log("Connected to host");
    }

    void OnDisconnectedFromHost()
    {
        Debug.Log("Disconnected from host");
    }

    void OnCommandReceived(string command, string senderId)
    {
        Debug.Log($"Received from {senderId}: {command}");
    }


    void OnDestroy()
    {
        host?.Stop();
        client?.Stop();
    }
}

// Host implementation
public class LANHost
{
    public event Action<string> OnClientConnected;
    public event Action<string> OnClientDisconnected;
    public event Action<string, string> OnCommandReceived;

    private TcpListener tcpListener;
    private UdpClient udpBroadcaster;
    private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
    private bool isRunning = false;
    private int port;
    private int discoveryPort;
    private Thread broadcastThread;
    private Thread clientListenerThread;

    public LANHost(int port, int discoveryPort)
    {
        this.port = port;
        this.discoveryPort = discoveryPort;
    }

    public void StartHost()
    {
        try
        {
            // Start TCP listener for clients
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            // Start UDP broadcaster for discovery
            udpBroadcaster = new UdpClient(discoveryPort);
            udpBroadcaster.EnableBroadcast = true;

            isRunning = true;

            // Start threads
            broadcastThread = new Thread(BroadcastLoop);
            broadcastThread.Start();

            clientListenerThread = new Thread(ClientListenerLoop);
            clientListenerThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start host: {e.Message}");
        }
    }

    void BroadcastLoop()
    {
        byte[] data = Encoding.UTF8.GetBytes($"HOST_AVAILABLE:{port}");
        IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);

        while (isRunning)
        {
            try
            {
                udpBroadcaster.Send(data, data.Length, broadcastEndpoint);
                Thread.Sleep(1000); // Broadcast every second
            }
            catch (Exception e)
            {
                if (isRunning)
                    Debug.LogError($"Broadcast error: {e.Message}");
            }
        }
    }

    void ClientListenerLoop()
    {
        while (isRunning)
        {
            try
            {
                TcpClient newClient = tcpListener.AcceptTcpClient();
                string clientId = Guid.NewGuid().ToString("N")[..8];
                clients[clientId] = newClient;

                OnClientConnected?.Invoke(clientId);

                // Start handling this client
                Thread clientThread = new Thread(() => HandleClient(clientId, newClient));
                clientThread.Start();
            }
            catch (Exception e)
            {
                if (isRunning)
                    Debug.LogError($"Client listener error: {e.Message}");
            }
        }
    }

    void HandleClient(string clientId, TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            while (isRunning && client.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnCommandReceived?.Invoke(command, clientId);
                }
                else
                {
                    break; // Client disconnected
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Client handling error: {e.Message}");
        }
        finally
        {
            clients.Remove(clientId);
            client.Close();
            OnClientDisconnected?.Invoke(clientId);
        }
    }

    public void SendCommandToClients(string command)
    {
        byte[] data = Encoding.UTF8.GetBytes(command);
        List<string> disconnectedClients = new List<string>();

        foreach (var kvp in clients)
        {
            try
            {
                if (kvp.Value.Connected)
                {
                    NetworkStream stream = kvp.Value.GetStream();
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                else
                {
                    disconnectedClients.Add(kvp.Key);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send to client {kvp.Key}: {e.Message}");
                disconnectedClients.Add(kvp.Key);
            }
        }

        // Clean up disconnected clients
        foreach (string clientId in disconnectedClients)
        {
            clients.Remove(clientId);
            OnClientDisconnected?.Invoke(clientId);
        }
    }

    public void Stop()
    {
        isRunning = false;

        tcpListener?.Stop();
        udpBroadcaster?.Close();

        foreach (var client in clients.Values)
        {
            client.Close();
        }
        clients.Clear();

        broadcastThread?.Join(1000);
        clientListenerThread?.Join(1000);
    }
}

// Client implementation
public class LANClient
{
    public event Action OnConnectedToHost;
    public event Action OnDisconnectedFromHost;
    public event Action<string, string> OnCommandReceived;

    private TcpClient tcpClient;
    private UdpClient udpListener;
    private NetworkStream stream;
    private bool isRunning = false;
    private bool isConnected = false;
    private int port;
    private int discoveryPort;
    private Thread discoveryThread;
    private Thread messageThread;

    public LANClient(int port, int discoveryPort)
    {
        this.port = port;
        this.discoveryPort = discoveryPort;
    }

    public void StartClient()
    {
        try
        {
            udpListener = new UdpClient(discoveryPort);
            isRunning = true;

            discoveryThread = new Thread(DiscoveryLoop);
            discoveryThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start client: {e.Message}");
        }
    }

    void DiscoveryLoop()
    {
        IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, discoveryPort);

        while (isRunning && !isConnected)
        {
            try
            {
                byte[] data = udpListener.Receive(ref remoteEndpoint);
                string message = Encoding.UTF8.GetString(data);

                if (message.StartsWith("HOST_AVAILABLE:"))
                {
                    string[] parts = message.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int hostPort))
                    {
                        ConnectToHost(remoteEndpoint.Address, hostPort);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                if (isRunning)
                    Debug.LogError($"Discovery error: {e.Message}");
                Thread.Sleep(100);
            }
        }
    }

    void ConnectToHost(IPAddress hostAddress, int hostPort)
    {
        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(hostAddress, hostPort);
            stream = tcpClient.GetStream();

            isConnected = true;
            OnConnectedToHost?.Invoke();

            // Start message handling thread
            messageThread = new Thread(MessageLoop);
            messageThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to host: {e.Message}");
        }
    }

    void MessageLoop()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (isRunning && isConnected && tcpClient.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnCommandReceived?.Invoke(command, "Host");
                }
                else
                {
                    break; // Host disconnected
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Message loop error: {e.Message}");
        }
        finally
        {
            isConnected = false;
            OnDisconnectedFromHost?.Invoke();
        }
    }

    public void SendCommandToHost(string command)
    {
        if (!isConnected || stream == null) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send command to host: {e.Message}");
            isConnected = false;
            OnDisconnectedFromHost?.Invoke();
        }
    }

    public void Stop()
    {
        isRunning = false;
        isConnected = false;

        tcpClient?.Close();
        udpListener?.Close();

        discoveryThread?.Join(1000);
        messageThread?.Join(1000);
    }
}