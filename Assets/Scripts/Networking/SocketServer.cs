using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class SocketServer : MonoBehaviour
    {
        private static SocketServer _instance;
        public static SocketServer Instance => _instance;
        private volatile bool _isRunning = false;
        private TcpListener _server;
        private TcpClient _currentClient;
        private NetworkStream _stream;
        private Thread _listenerThread;
        private StreamWriter _writer;
        
        public Action<NetworkMessage> OnMessageReceived;
        
        private const int Port = 1337;
        
        [SerializeField] private Text ipText;
        
        [SerializeField] private GameStartMenu gameStartMenu;

        private void Start()
        {
            string localIP = GetLocalIPAddress();
            Debug.Log("Local IP Address: " + localIP);
            
            if (ipText != null)
                ipText.text = localIP;
            
            if (gameStartMenu!=null && gameStartMenu.simulateConnection)
            {
                return;
            }

            _listenerThread = new Thread(StartServer)
            {
                IsBackground = true
            };
            _listenerThread.Start();

        }

        private void Update()
        {
            Debug.Log("Update");
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject); 
        }

        private void StartServer()
        {
            _server = new TcpListener(IPAddress.Any, Port);
            _server.Start();
            Debug.Log("TCP server listening on port " + Port);
            
            _isRunning = true;

            try
            {
                _currentClient = _server.AcceptTcpClient();
                _stream = _currentClient.GetStream();
                _writer = new StreamWriter(_stream, Encoding.ASCII);
                _writer.AutoFlush = true;

                Debug.Log("Client connected: " + _currentClient.Client.RemoteEndPoint);
            
                if (gameStartMenu != null)
                {
                    MessageDispatcher.Instance.Enqueue(() =>
                    {
                        gameStartMenu.OnConnectionEstablished();
                    });
                }


                byte[] buffer = new byte[1024];
                int bytesRead;

                while (_isRunning && (bytesRead = _stream.Read(buffer, 0, buffer.Length)) != 0) {
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("📥 Received: " + json);

                    try {
                        Debug.Log("Parsing JSON...");
                        NetworkMessage msg = JsonUtility.FromJson<NetworkMessage>(json);
                        MessageDispatcher.Instance.Enqueue(() => {
                            Debug.Log("Enqueued message for main thread");
                            OnMessageReceived?.Invoke(msg);
                        });

                    } catch (Exception e) {
                        Debug.LogWarning("Failed to parse JSON: " + e.Message);
                    }
                }
            }
            catch (IOException)
            {
                Debug.Log("Server shutdown.");
            }
            finally
            {
                _isRunning = false;
            }
            

        }
        
        private void OnDestroy()
        {
            Debug.LogWarning("[SocketServer] OnDestroy llamado — El objeto fue destruido");
        }

        
        public void SendMessageToClient(NetworkMessage message) {
            if (_writer == null) {
                Debug.LogWarning("Cannot send: writer not ready");
                return;
            }

            string json = JsonUtility.ToJson(message);
            _writer.WriteLine(json);
//            Debug.Log("📤 Sent to client: " + json);

        }
        
        public bool isClientConnected => _currentClient is { Connected: true } && _stream is { CanWrite: true };

        string GetLocalIPAddress()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                        ni.Description.ToLower().Contains("virtual") ||
                        ni.Description.ToLower().Contains("pseudo") ||
                        ni.Name.ToLower().Contains("vmware") ||
                        ni.Name.ToLower().Contains("tailscale"))
                        continue;

                    var ipProps = ni.GetIPProperties();

                    foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            string addr = ip.Address.ToString();
                            if (addr.StartsWith("192.168.") || addr.StartsWith("10.") || addr.StartsWith("172."))
                            {
                                return addr;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al obtener la IP local: " + ex.Message);
            }

            return "No se pudo obtener la IP";
        }

    
        void OnApplicationQuit() {
            _isRunning = false;

            try
            {
                _stream?.Close();
                _currentClient?.Close();
                _server?.Stop();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error shutting down server: " + e.Message);
            }
            
            if (_listenerThread != null && _listenerThread.IsAlive)
                _listenerThread.Join();
        }

        
    }
}
