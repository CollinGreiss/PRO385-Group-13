using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

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
    public int port = 12345;
    public string serverIP = "10.10.15.38"; // localhost for testing

    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private Thread serverThread;
    private Thread clientThread;
    private bool isServer = false;
    private bool isConnected = false;

    public event Action<string> CommandReceived;


    public void StartHost()
    {
        try
        {
            isServer = true;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            Debug.Log("Server started, waiting for connection...");

            serverThread = new Thread(ServerLoop);
            serverThread.Start();

        }
        catch (Exception e)
        {
            Debug.Log($"Server error: {e.Message}");
        }
    }

    public void StartClient()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, port);
            stream = client.GetStream();

            Debug.Log("Connected to server!");
            isConnected = true;

            clientThread = new Thread(ClientLoop);
            clientThread.Start();

        }
        catch (Exception e)
        {
            Debug.Log($"Connection error: {e.Message}");
        }
    }

    private void ServerLoop()
    {
        try
        {
            using (TcpClient serverClient = server.AcceptTcpClient())
            {
                Debug.Log("Client connected!");
                stream = serverClient.GetStream();
                isConnected = true;

                /* Enable send button on main thread
                UnityMainThreadDispatcher.Instance.Enqueue(() => {
                    sendButton.interactable = true;
                });
                */

                byte[] buffer = new byte[1024];
                while (isConnected && serverClient.Connected)
                {
                    try
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            UnityMainThreadDispatcher.Instance.Enqueue(() =>
                            {
                                Debug.Log($"Client: {message}");
                                CommandReceived?.Invoke(message);
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            Debug.Log($"Read error: {e.Message}");
                        });
                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                Debug.Log($"Server error: {e.Message}");
            });
        }
    }

    private void ClientLoop()
    {
        byte[] buffer = new byte[1024];
        while (isConnected && client.Connected)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Debug.Log($"Server: {message}");
                        CommandReceived?.Invoke(message);
                    });
                }
            }
            catch (Exception e)
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    Debug.Log($"Read error: {e.Message}");
                });
                break;
            }
        }
    }

    public void SendCommand(string message)
    {
        if (!isConnected || stream == null) return;

        if (string.IsNullOrEmpty(message)) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            string sender = isServer ? "You (Server)" : "You (Client)";
            Debug.Log($"{sender}: {message}");

        }
        catch (Exception e)
        {
            Debug.LogError($"Send error: {e.Message}");
        }
    }

    private void OnDestroy()
    {
        isConnected = false;

        if (serverThread != null && serverThread.IsAlive)
            serverThread.Abort();
        if (clientThread != null && clientThread.IsAlive)
            clientThread.Abort();

        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
        if (server != null)
            server.Stop();
    }
}

// Helper class for threading
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    public static UnityMainThreadDispatcher Instance => _instance;

    private System.Collections.Generic.Queue<System.Action> _executionQueue = new System.Collections.Generic.Queue<System.Action>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    public void Enqueue(System.Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}