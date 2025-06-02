using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    public GameObject gameLogo;
    [Header("UI Pages")] 
    public GameObject connectionPanel; 
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;
    
    [Header("Connection Panel")]
    public Text statusText;
    public Image statusIcon;

    public List<Button> returnButtons;
    
    public bool simulateConnection = false;


    // Start is called before the first frame update
    void Start()
    {
        EnableConnectionPanel();

        //Hook events
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
        
        if (simulateConnection)
        {
            StartCoroutine(SimulateCompanionConnection());
        }
    }
    
    void Update()
    {
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            OnConnectionEstablished();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        AudioManager.instance.StopWithFade("uimenubgm", 3f);
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }
    private IEnumerator StartGameRoutine()
    {
        HideAll();
        AudioManager.instance.StopWithFade("uimenubgm", 3f);

        yield return new WaitForSeconds(2f);

        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }


    public void HideAll()
    {
        gameLogo.SetActive(false);
        connectionPanel.SetActive(false);
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableConnectionPanel()
    {
        connectionPanel.SetActive(true);
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }
    
    public void OnConnectionEstablished()
    {
        StartCoroutine(
            BlinkConnectionStatus(
                () => StartCoroutine(
                    HandleConnectionSequence()
                    )
                )
            );
    }
    
    private IEnumerator HandleConnectionSequence()
    {
        CanvasGroup cg = connectionPanel.GetComponent<CanvasGroup>();
        CanvasGroup menuCg = mainMenu.GetComponent<CanvasGroup>();

        float duration = 1.5f;


        if (cg != null)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }

            cg.alpha = 0f;
        }
        
        connectionPanel.SetActive(false);
        
        if (menuCg != null)
        {
            menuCg.interactable = false;
            mainMenu.SetActive(true);
            menuCg.alpha = 0f;

            Transform menuTransform = mainMenu.transform;
            Vector3 initialScale = Vector3.one * 0.85f;
            Vector3 targetScale = Vector3.one;
            menuTransform.localScale = initialScale;

            float elapsed = 0f;
            
            
            AudioManager.instance.PlayWithFade("uimenubgm", 2f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                menuCg.alpha = Mathf.Lerp(0f, 1f, t);
                menuTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);

                yield return null;
            }

            menuCg.alpha = 1f;
            menuTransform.localScale = targetScale;
        }

        menuCg.interactable = true;
        
        
        EnableMainMenu();
        
        /*
        if (SocketServer.Instance != null && SocketServer.Instance.isClientConnected)
        {
            NetworkMessage msg = new NetworkMessage
            {
                type = "event",
                payload = new Payload
                {
                    name = "connection_established",
                    room = "intro" // o lo que sea relevante
                }
            };

            SocketServer.Instance.SendMessageToClient(msg);
        }*/
    }
    
    private IEnumerator BlinkConnectionStatus(Action onComplete)
    {
        statusText.text = "Status: Conectado";
        Color originalTextColor = statusText.color;
        Color blinkColor = Color.green;

        Color originalIconColor = statusIcon.color;

        float duration = 2f;
        float blinkInterval = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            bool isVisible = Mathf.FloorToInt(elapsed / blinkInterval) % 2 == 0;

            statusText.color = isVisible ? blinkColor : originalTextColor;
            statusIcon.color = isVisible ? blinkColor : originalIconColor;

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        statusText.color = blinkColor;
        statusIcon.color = blinkColor;
        
        AudioManager.instance.Play("uiconnectionready");
        AudioManager.instance.StopWithFade("bgm", 2f);

        onComplete?.Invoke();
    }
    
    private IEnumerator SimulateCompanionConnection()
    {
        yield return new WaitForSeconds(5f); 
        OnConnectionEstablished();
    }



}
