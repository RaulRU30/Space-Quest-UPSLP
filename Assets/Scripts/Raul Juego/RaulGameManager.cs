using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaulGameManager : MonoBehaviour
{

    [SerializeField] private List<int> correctSequence = new List<int> {1, 3, 2, 4};
    private List<int> currentSequence = new List<int>();
    
    public void OnButtonPressed(int id)
    {
        currentSequence.Add(id);

        int currentIndex = currentSequence.Count - 1;
        if (currentSequence[currentIndex] != correctSequence[currentIndex])
        {
            Debug.Log("❌ Secuencia incorrecta. Reiniciando...");
            currentSequence.Clear();
            return;
        }

        if (currentSequence.Count == correctSequence.Count)
        {
            Debug.Log("✅ ¡Secuencia correcta!");
            AudioManager.instance.Play("uiconnectionready");
            currentSequence.Clear();
        }
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
