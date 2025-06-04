using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Networking;
using UnityEngine;

public class GameManagerServer : MonoBehaviour
{
    [SerializeField] public Transform playerTransform;
    [SerializeField] private float positionUpdateInterval = 0.2f;
    [SerializeField] private float positionThreshold = 0.05f;
    [SerializeField] private float rotationThreshold = 1f;
    public CodeTest codeTest;
    
    private Vector3 _lastSentPosition;
    private Vector3 _lastSentEulerRotation;
    private float _timer = 0f;
    void Start () {
        
        if(SocketServer.Instance == null) {
            Debug.LogError("SocketServer instance is not initialized. Make sure to start the server first.");
            return;
        }

        SocketServer.Instance.OnMessageReceived += (msg) =>
        {
            MessageDispatcher.Instance.Enqueue(() => HandleClientMessage(msg));
        };
    }


    private void Update() {
        if (!SocketServer.Instance.isClientConnected || playerTransform == null) return;
        
        _timer += Time.deltaTime;
        
        if (_timer >= positionUpdateInterval) {
            Vector3 currentPosition = playerTransform.position;
            Vector3 currentRotation = playerTransform.eulerAngles;
            
            bool moved = Vector3.Distance(_lastSentPosition, currentPosition) > positionThreshold;
            bool rotated = Vector3.Distance(_lastSentEulerRotation, currentRotation) > rotationThreshold;

            if (moved || rotated) {
                SendPlayerTransform(currentPosition, currentRotation);
                _lastSentPosition = currentPosition;
                _lastSentEulerRotation = currentRotation;
            }
            
            _timer = 0f;
        }
    }
    
    private void SendPlayerTransform(Vector3 position, Vector3 rotation) {
        var msg = new NetworkMessage {
            type = "position",
            payload = new Payload {
                x = position.x,
                y = position.y,
                z = position.z,
                rotationX = rotation.x,
                rotationY = rotation.y,
                rotationZ = rotation.z
            }
        };

        SocketServer.Instance.SendMessageToClient(msg);
    }
    public void SendIndexCode(int NumeroEstado, int index){
        var msg = new NetworkMessage
        {
            type = "IndexCode",
            payload = new Payload
            {
                state = NumeroEstado,
                codeindex = index,

            }
        };
        SocketServer.Instance.SendMessageToClient(msg);
    }
    public void SendComplateTask(int state)
    {
        var msg = new NetworkMessage
        {
            type = "TaskComplate",
            payload = new Payload
            {
                state = state,

            }
        };
        SocketServer.Instance.SendMessageToClient(msg);
    }
    public void SendTextCode(String codeText)
    {
        var msg = new NetworkMessage
        {
            type = "GeneratorCode",
            payload = new Payload
            {
                code = codeText,
            }
        };
        SocketServer.Instance.SendMessageToClient(msg);
    }
    
    void HandleClientMessage(NetworkMessage message) {
        Debug.Log("📩 Handling client message of type: " + message.type);

        switch (message.type) {
            case "command":
                Debug.Log($"⚙️ Command received: {message.payload.action} on {message.payload.target}");
                HandleCommand(message.payload.action, message.payload.target);
                break;
            case "StartCode":
                codeTest.GenerateCode();
                break;
            default:
                Debug.Log("❓ Unknown message type: " + message.type);
                break;
        }
    }
    
    void HandleCommand(string action, string target)
    {
        switch (action)
        {
            case "start_key_game":
                Debug.Log("🟢 Recibido: start_key_game");

                if (KeyManager.Instance == null)
                {
                    Debug.LogError("❌ KeyManagerVR.Instance es null");
                }
                else
                {
                    KeyManager.Instance.MostrarTodasLasLlaves();
                    Debug.Log("✅ Llaves mostradas desde GameManagerServer");
                }

                break;
            
        }
    }

    public void SendKeyCollectedMessage(string keyId)
    {
        var msg = new NetworkMessage
        {
            type = "event",
            payload = new Payload
            {
                action = "collect_key",
                target = keyId
            }
        };

        SocketServer.Instance.SendMessageToClient(msg);
        Debug.Log($"📨 Enviado al Companion: {keyId}");
    }
    

}
