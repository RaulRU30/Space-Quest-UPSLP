using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Networking
{
    public class SocketServer : MonoBehaviour
    {
        private TcpListener _server;
        private TcpClient _currentClient;
        private NetworkStream _stream;
        private Thread _listenerThread;
        private StreamWriter _writer;
        
        public Action<NetworkMessage> OnMessageReceived;
        
        private const int Port = 1337;
        
        [SerializeField] private TextMeshPro ipText;

        private void Start()
        {
            string localIP = GetLocalIPAddress();
            Debug.Log("IP local del dispositivo: " + localIP);
            
            if (ipText != null)
                ipText.text = localIP;

            _listenerThread = new Thread(StartServer)
            {
                IsBackground = true
            };
            _listenerThread.Start();
        }
    
        private void StartServer()
        {
            _server = new TcpListener(IPAddress.Any, Port);
            _server.Start();
            Debug.Log("🟢 TCP server listening on port " + Port);

            _currentClient = _server.AcceptTcpClient();
            _stream = _currentClient.GetStream();
            _writer = new StreamWriter(_stream, Encoding.ASCII);
            _writer.AutoFlush = true;


            Debug.Log("🔗 Client connected: " + _currentClient.Client.RemoteEndPoint);

            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) != 0) {
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
        
        public void SendMessageToClient(NetworkMessage message) {
            if (_writer == null) {
                Debug.LogWarning("Cannot send: writer not ready");
                return;
            }

            string json = JsonUtility.ToJson(message);
            _writer.WriteLine(json);
            //Debug.Log("📤 Sent to client: " + json);

        }
        
        public bool isClientConnected => _currentClient is { Connected: true } && _stream is { CanWrite: true };

        string GetLocalIPAddress()
        {
            string localIP = "No se pudo obtener la IP";
            try
            {
                foreach (var host in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (host.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = host.ToString();
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error al obtener la IP: " + ex.Message);
            }

            return localIP;
        }

        public void StopServer()
        {
            try
            {
                Debug.Log("🛑 Stopping server...");

                if (_writer != null)
                {
                    _writer.Close();
                    _writer = null;
                }

                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                if (_currentClient != null)
                {
                    _currentClient.Close();
                    _currentClient = null;
                }

                if (_server != null)
                {
                    _server.Stop();
                    _server = null;
                }

                if (_listenerThread != null && _listenerThread.IsAlive)
                {
                    _listenerThread.Abort(); // Optional — if you're not using a cancellation flag
                    _listenerThread = null;
                }

                Debug.Log("✅ Server stopped successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Error stopping server: " + e.Message);
            }
        }


    
        void OnApplicationQuit() {
            _server?.Stop();
            _listenerThread?.Abort();
        }
        
    }
}
