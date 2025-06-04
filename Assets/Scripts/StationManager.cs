using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StationManager : MonoBehaviour
{
    [Header("Station Setup")]
    public List<Station> stations;

    [Header("Station Icons")]
    public List<Sprite> stationSprites;

    private Station currentTargetStation;
    private int currentTargetIndex = -1;

    public Sprite successSprite;
    public Sprite failureSprite;
    private void Start()
    {
        // Initialize all stations
        foreach (Station station in stations)
        {
            station.Initialize(this);
        }

        SetNextStation();
    }

    public void OnStationPressed(Station pressedStation)
    {
        if (pressedStation == currentTargetStation)
        {
            Debug.Log("✅ Correct station pressed!");

            if (currentTargetIndex+1 >= stationSprites.Count)
            {
                Debug.Log("🎉 Secuencia completada" + stationSprites.Count);
                ShowSuccess();
                currentTargetIndex = 0;
            }
            else
            {
                Debug.Log("🎉 Secuencia completada" + stationSprites.Count+" "+currentTargetIndex);
                SetNextStation();
            }
        }   
        else
        {
            Debug.Log("❌ Wrong station. Restarting...");
            ShowFailure();
            Invoke(nameof(RestartSequence), 2f);
        }
    }

    private void SetNextStation()
    {
        currentTargetIndex = (currentTargetIndex + 1) % stations.Count;
        currentTargetStation = stations[currentTargetIndex];

        Sprite targetSprite = stationSprites[currentTargetIndex];
        Debug.Log($"Asignando sprite: {targetSprite.name}");
        foreach (Station station in stations)
        {
            station.SetTopImage(targetSprite);
        }

        Debug.Log($"➡️ New target: {currentTargetStation.stationName}");
    }

    private void RestartSequence()
    {
        currentTargetIndex = -1;
        SetNextStation();
    }
    private void ShowSuccess()
    {
        foreach (var station in stations)
        {
            station.SetTopImage(successSprite);
        }
    }

    private void ShowFailure()
    {
        foreach (var station in stations)
        {
            station.SetTopImage(failureSprite);
        }
    }
}
