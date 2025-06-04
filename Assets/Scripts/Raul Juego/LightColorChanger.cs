using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LightColorChanger : MonoBehaviour
{
    [Header("Light 1")]
    public Renderer wallRenderer;
    public Light pointLight;
    
    [FormerlySerializedAs("wallRenderer_light2")] [Header("Light 2")]
    public Renderer wallRendererLight2;
    [FormerlySerializedAs("pointLight_light2")] public Light pointLightLight2;

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
        
        if (wallRendererLight2 != null)
        {
            Material[] materials2 = wallRendererLight2.materials;
            if (materialIndexToChange >= 0 && materialIndexToChange < materials2.Length)
            {
                materials2[materialIndexToChange].SetColor(colorProperty, newColor);
                wallRendererLight2.materials = materials2;
            }
            else
            {
                Debug.LogWarning("Índice de material inválido (Light 2).");
            }
        }

        if (pointLightLight2 != null)
        {
            pointLightLight2.color = newColor;
        }
    }

}
