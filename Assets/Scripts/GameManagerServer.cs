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
    
    private Vector3 _lastSentPosition;
    private Vector3 _lastSentEulerRotation;
    private float _timer = 0f;
    void Start () {

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

            default:
                Debug.Log("❓ Unknown message type: " + message.type);
                break;
        }
    }
    
    void HandleCommand(string action, string target)
    {
        switch (action)
        {
            case "close_nearest_door":
                CloseNearestDoor();
                break;
            case "open_door":
            case "close_door":
            {
                bool open = action == "open_door";

                DoorController[] doors = FindObjectsOfType<DoorController>();
                foreach (var door in doors)
                {
                    if (door.GetDoorId() != target) continue;
                    door.SetDoorState(open);

                    if (!open) // si se cerró, reabrir luego
                        StartCoroutine(OpenDoorAfterDelay(door, 3f));

                    return;
                }

                break;
            }
        }
    }

    private void CloseNearestDoor()
    {
        DoorController[] doors = FindObjectsOfType<DoorController>();
        if (doors.Length == 0 || playerTransform == null)
        {
            Debug.LogWarning("❌ No doors or player transform available.");
            return;
        }

        DoorController closest = null;
        float minDistance = float.MaxValue;
        Vector3 playerPos = playerTransform.position;

        foreach (var door in doors)
        {
            float dist = Vector3.Distance(door.transform.position, playerPos);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = door;
            }
        }

        if (closest != null)
        {
            Debug.Log($"🔒 Closest door to close: {closest.GetDoorId()} at {minDistance:F2}m");
            closest.SetDoorState(false); // cerrar
            StartCoroutine(OpenDoorAfterDelay(closest, 3f)); // reabrir luego
        }
    }

    private IEnumerator OpenDoorAfterDelay(DoorController door, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        door.SetDoorState(true);
        Debug.Log("🚪 Door reopened automatically.");
    }

    private void CloseAllDoors()
    {
        DoorController[] doors = FindObjectsOfType<DoorController>();
        foreach (var door in doors)
        {
            door.SetDoorState(false);
        }
        Debug.Log($"🔒 {doors.Length} doors closed by remote command.");
    }

}
