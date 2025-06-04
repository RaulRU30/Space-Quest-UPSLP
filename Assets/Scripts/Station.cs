using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour
{
    [Header("References")]
    public Image console_ceiling_Image;  // consolaCheck > console_ceiling > Canvas > Image
    public Button console_Button;        // consolaCheck > console > Canvas > Button

    public string stationName;

    private StationManager stationManager;

    public void Initialize(StationManager manager)
    {
        stationManager = manager;
    }

    // This sets the top screen image (console_ceiling)
    public void SetTopImage(Sprite sprite)
    {
        console_ceiling_Image.sprite = sprite;
    }

    public void OnPressed()
    {
        Debug.Log($"{stationName} fue presionado");
        stationManager.OnStationPressed(this);
    }
}

