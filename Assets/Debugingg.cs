using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Debugingg : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;

    void Update()
    {
        if (moveAction == null || moveAction.action == null) return;

        Vector2 move = moveAction.action.ReadValue<Vector2>();
        if (move != Vector2.zero)
            Debug.Log("🕹️ Move input received: " + move);
    }
}
