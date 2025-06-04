using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColorChanger : MonoBehaviour
{
    [Header("Referencias")]
    public Renderer wallRenderer;
    public Light pointLight;

    [Header("Material Info")]
    public int materialIndexToChange = 1;
    public string colorProperty = "_Color";
    
    public void ChangeColorToGreen()
    {
        ChangeColor(Color.green);
    }
    
    public void ChangeColorToRed()
    {
        ChangeColor(Color.red);
    }
    
    public void ChangeColorToBlue()
    {
        ChangeColor(Color.blue);
    }
    
    public void ChangeColorToYellow()
    {
        ChangeColor(Color.yellow);
    }
    
    public void ChangeColor(Color newColor)
    {
        if (wallRenderer != null)
        {
            Material[] materials = wallRenderer.materials;
            if (materialIndexToChange >= 0 && materialIndexToChange < materials.Length)
            {
                materials[materialIndexToChange].SetColor(colorProperty, newColor);
                wallRenderer.materials = materials;
            }
            else
            {
                Debug.LogWarning("Índice de material inválido.");
            }
        }
        if (pointLight != null)
        {
            pointLight.color = newColor;
        }
    }

}
