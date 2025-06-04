using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushButton : MonoBehaviour
{
    public int buttonId;
    public UnityEvent<int> onButtonPressed;
    
    public void Press()
    {
        onButtonPressed.Invoke(buttonId);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
