using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberButton : MonoBehaviour
{
    public int assignedNumber { get; private set; }
    
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject visualTarget;
    
    public void SetNumber(int number)
    {
        assignedNumber = number;
        label.text = number.ToString();
        Debug.Log($"[NumberButton] SetNumber: {assignedNumber} en {gameObject.name}");

    }

    public void OnButtonPressed()
    {
        Debug.Log($"🖱️ Botón presionado: {assignedNumber} ({gameObject.name})");

        SequenceNumberGameManager.Instance.ValidateNumber(this);
    }
    
    public void EnableButton(bool enable)
    {
        if (visualTarget != null)
            visualTarget.SetActive(enable);
        gameObject.SetActive(enable);
    }


}
