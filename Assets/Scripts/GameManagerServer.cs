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
    
    private SocketServer _socket;
    private Vector3 _lastSentPosition;
    private Vector3 _lastSentEulerRotation;
    private float _timer = 0f;
    
    void Awake() {
        _socket = GetComponent<SocketServer>();

        if (_socket == null) {
            Debug.LogError("Missing SocketServer component.");
            return;
        }

        _socket.OnMessageReceived += (msg) =>
        {
            MessageDispatcher.Instance.Enqueue(() => HandleClientMessage(msg));
        };
    }


    private void Update() {
        if (!_socket.isClientConnected || playerTransform == null) return;
        
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

        _socket.SendMessageToClient(msg);
    }

    
    void HandleClientMessage(NetworkMessage message) {
        Debug.Log("📩 Handling client message of type: " + message.type);

        switch (message.type) {
            case "command":
                Debug.Log($"⚙️ Command received: {message.payload.action} on {message.payload.target}");
                HandleCommand(message.payload.action, message.payload.target);
                break;

            default:
                Debug.Log("❓ Unknown message type: " + message.type);
                break;
        }
    }
    
    void HandleCommand(string action, string target)
    {
        if (action != "open_door" && action != "close_door") return;
        
        bool open = action == "open_door";

        Debug.Log("🔑 Searching for door with ID: " + target + " to " + (open ? "open" : "close") + " it");
        DoorController[] doors = FindObjectsOfType<DoorController>();

        Debug.Log("Doors found : " + doors.Length);

        foreach (var door in doors)
        {
            Debug.Log("Door ID: " + door.GetDoorId());
            if (door.GetDoorId() != target) continue;
            door.SetDoorState(open);
            return;
        }

        Debug.LogWarning("🚫 Door not found: " + target);

    }


}
