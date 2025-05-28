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
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float bombSpawnHeight = 5f;
    private SocketServer _socket;
    private Vector3 _lastSentPosition;
    private Vector3 _lastSentEulerRotation;
    private float _timer = 0f;

    void Awake()
    {
        _socket = GetComponent<SocketServer>();

        if (_socket == null)
        {
            Debug.LogError("Missing SocketServer component.");
            return;
        }

        _socket.OnMessageReceived += (msg) =>
        {
            MessageDispatcher.Instance.Enqueue(() => HandleClientMessage(msg));
        };
    }


    private void Update()
    {
        if (!_socket.isClientConnected || playerTransform == null) return;

        _timer += Time.deltaTime;

        if (_timer >= positionUpdateInterval)
        {
            Vector3 currentPosition = playerTransform.position;
            Vector3 currentRotation = playerTransform.eulerAngles;

            bool moved = Vector3.Distance(_lastSentPosition, currentPosition) > positionThreshold;
            bool rotated = Vector3.Distance(_lastSentEulerRotation, currentRotation) > rotationThreshold;

            if (moved || rotated)
            {
                SendPlayerTransform(currentPosition, currentRotation);
                _lastSentPosition = currentPosition;
                _lastSentEulerRotation = currentRotation;
            }


            _timer = 0f;
        }
    }

    private void SendPlayerTransform(Vector3 position, Vector3 rotation)
    {
        var msg = new NetworkMessage
        {
            type = "position",
            payload = new Payload
            {
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


    void HandleClientMessage(NetworkMessage message)
    {
        Debug.Log("📩 Handling client message of type: " + message.type);

        switch (message.type)
        {
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
        if (action == "close_nearest_door")
        {
            CloseNearestDoor();
        }
        else if (action == "open_door" || action == "close_door")
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
        }
        else if (action == "drop_bomb")
        {
            DropBombOnPlayer();
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

    private void DropBombOnPlayer()
    {
        if (playerTransform == null || bombPrefab == null)
        {
            Debug.LogWarning("❌ Cannot drop bomb — missing playerTransform or bombPrefab.");
            return;
        }

        StartCoroutine(DropBombWithDelay(6f));
    }
    private IEnumerator DropBombWithDelay(float delaySeconds)
{
    Debug.Log("⏳ Bomb will drop in " + delaySeconds + " seconds...");

    yield return new WaitForSeconds(delaySeconds); // Espera

    Vector3 spawnPosition = playerTransform.position;
    Instantiate(bombPrefab, spawnPosition, Quaternion.identity);

    Debug.Log("💣 Bomb dropped on player after delay!");
}
}
