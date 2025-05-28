using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalCollectibles = 3;
    private int collectedCount = 0;

    public GameObject winUI;
    public AudioClip winSound;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectItem()
    {
        collectedCount++;
        Debug.Log($"✅ Collected {collectedCount} / {totalCollectibles}");

        if (collectedCount >= totalCollectibles)
        {
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        Debug.Log("🏆 All items collected! You win!");

        if (winSound != null)
            AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);

        if (winUI != null)
            winUI.SetActive(true);
    }
}
