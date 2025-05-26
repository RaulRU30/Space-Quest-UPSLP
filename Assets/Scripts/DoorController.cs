using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private string doorId;

    private Animator _animator;
    private static readonly int CharacterNearby = Animator.StringToHash("character_nearby");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Missing Animator component on the door.");
        }
    
    }
    
    void OnEnable() {
        Debug.Log("✅ DoorController enabled on: " + doorId);
    }

    
    public string GetDoorId() => doorId;


    public void SetDoorState(bool isOpen)
    {
        _animator.SetBool(CharacterNearby, isOpen);
        Debug.Log("Door state changed: " + (isOpen ? "Open" : "Open") + " for door ID: " + doorId);
    }
    
    [ContextMenu("Open Door")]
    public void OpenDoorContext() {
        SetDoorState(true);
    }

    [ContextMenu("Close Door")]
    public void CloseDoorContext() {
        SetDoorState(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        OpenAllDoorsAtStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OpenAllDoorsAtStart()
    {
        DoorController[] doors = FindObjectsOfType<DoorController>();
        foreach (var door in doors)
        {
            door.SetDoorState(true);
        }
        Debug.Log($"🚪 {doors.Length} doors opened at start.");
    }
}
