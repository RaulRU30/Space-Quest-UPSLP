using UnityEngine;
using UnityEngine.SceneManagement;
using Networking; 

public class WinScreenUI : MonoBehaviour
{
    public void RestartGame()
    {
        // Stop server cleanly
        SocketServer server = FindObjectOfType<SocketServer>();
        if (server != null)
        {
            Debug.Log("dddd");
            server.StopServer();
        }

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

