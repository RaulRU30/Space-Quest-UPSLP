using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Networking
{
    public class ServerDiscoveryResponder : MonoBehaviour
    {
        private UdpClient _udpServer;
        private const int Port = 8888;

        private void Start()
        {
            _udpServer = new UdpClient(Port);
            _udpServer.EnableBroadcast = true;

            Debug.Log("UDP discovery responder running on port " + Port);
            _udpServer.BeginReceive(OnBroadcastReceived, null);

        }

        private void OnBroadcastReceived(IAsyncResult result) {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, Port);
            byte[] data = _udpServer.EndReceive(result, ref sender);
            string message = Encoding.UTF8.GetString(data);

            if (message == "hello_server") {
                Debug.Log($"Discovery request received from: {sender.Address}");
                byte[] response = Encoding.UTF8.GetBytes("server_here");
                _udpServer.Send(response, response.Length, sender);
            }

            _udpServer.BeginReceive(OnBroadcastReceived, null);
        }

        private void OnApplicationQuit() {
            _udpServer?.Close();
        }
    }
}
