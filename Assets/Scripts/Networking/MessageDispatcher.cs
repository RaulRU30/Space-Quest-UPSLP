using System;
using System.Collections.Concurrent;
using Unity.VisualScripting;
using UnityEngine;

namespace Networking
{
    public class MessageDispatcher : MonoBehaviour
    {
        public static MessageDispatcher Instance { get; private set; }

        private readonly ConcurrentQueue<Action> _mainThreadQueue = new();

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("MessageDispatcher initialized.");
        }

        public void Enqueue(Action action) {
            _mainThreadQueue.Enqueue(action);
        }

        private void Update() {
            while (_mainThreadQueue.TryDequeue(out var action)) {
                try {
                    action?.Invoke();
                } catch (Exception e) {
                    Debug.LogError("❌ Error in main thread action: " + e.Message);
                }
            }
        }


    }
}
