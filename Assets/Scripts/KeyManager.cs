using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;
    public List<GameObject> llaves; // Asignar en inspector

    void Awake()
    {
        Instance = this;

        foreach (GameObject key in llaves)
        {
            if (key != null)
            {
                key.SetActive(false);
            }
        }
    }

    public void MostrarTodasLasLlaves()
    {
        foreach (GameObject key in llaves)
        {
            if (key != null)
            {
                key.SetActive(true);
                Debug.Log($"✅ Mostrando: {key.name}");
            }
        }
    }
}
