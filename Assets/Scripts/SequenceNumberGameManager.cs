using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SequenceNumberGameManager : MonoBehaviour
{
    public static SequenceNumberGameManager Instance;
    [SerializeField] private List<NumberButton> numberButtons;
    [SerializeField] private Text countdownText;
    [SerializeField] private Text statusText;
    [SerializeField] private float roundTime = 10f;
    
    [SerializeField] private Slider countdownSlider;
    [SerializeField] private LightColorChanger lightColorChanger;
    
    private int _currentBase = 1;
    private int _nextExpected;
    private float _timeRemaining;
    private bool _isGameActive = false;





    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        countdownText.text = "";
        lightColorChanger.ChangeColor(Color.white); 
    }
    
    public void StartGame()
    {
        _currentBase = 1;
        _nextExpected = _currentBase;
        _timeRemaining = roundTime;
        _isGameActive = true;
        countdownText.text = _timeRemaining.ToString("F2");
        
        countdownSlider.maxValue = roundTime;
        countdownSlider.value = roundTime;

        AssignNumbers();
        
        lightColorChanger.ChangeColorToBlue(); 
        statusText.text = $"Presiona el número {_nextExpected}";

        

    }
    
    private void Update()
    {
        if (!_isGameActive) return;

        _timeRemaining -= Time.deltaTime;
        countdownText.text = Mathf.CeilToInt(_timeRemaining).ToString();
        
        countdownSlider.value = _timeRemaining;


        if (_timeRemaining <= 0)
        {
            TimeOutGame();
        }
        
    }


    void AssignNumbers()
    {
        statusText.text = $"Ronda nueva. Presiona el número {_nextExpected}";

        List<int> numbers = new List<int>();
        for (int i = _currentBase; i < _currentBase + 4; i++)
        {
            numbers.Add(i);
        }

        Shuffle(numbers);

        for (int i = 0; i < numberButtons.Count; i++)
        {
            numberButtons[i].SetNumber(numbers[i]);
            numberButtons[i].EnableButton(true);
        }

        Debug.Log($"🔢 Nueva ronda: {_currentBase} al {_currentBase + 3}");
    }

    public void ValidateNumber(NumberButton button)
    {
        Debug.Log("🔍 Validando número: " + button.assignedNumber + " (Esperado: " + _nextExpected );
        if (!_isGameActive) return;

        if (button.assignedNumber == _nextExpected)
        {
            Debug.Log($"✅ Correcto: {_nextExpected}");
            AudioManager.instance.Play("uihoverexit");
            _nextExpected++;
            statusText.text = $"Correcto. Ahora presiona el número {_nextExpected}";

            if (_nextExpected <= _currentBase + 3) return;
            if (_currentBase + 3 >= 12)
            {
                statusText.text = $"Correcto. Ahora presiona el número {_nextExpected}";

                WinGame();
            }
            else
            {
                _currentBase += 4;
                _nextExpected = _currentBase;
                _timeRemaining = roundTime;
                AssignNumbers();
            }
        }
        else
        {
            Debug.LogWarning($"❌ Incorrecto. Se esperaba: {_nextExpected}");
            statusText.text = $"❌ Incorrecto. Se esperaba el número {_nextExpected}";
            StartCoroutine(HandleIncorrectAnswer());
        }
    }
    
    IEnumerator HandleIncorrectAnswer()
    {
        _isGameActive = false;
        
        for (int i = 0; i < 3; i++)
        {
            lightColorChanger.ChangeColorToRed();
            yield return new WaitForSeconds(0.2f);
            lightColorChanger.ChangeColor(Color.white);
            yield return new WaitForSeconds(0.2f);
        }
        
        _nextExpected = _currentBase;
        _timeRemaining = roundTime;
        AssignNumbers();
        _isGameActive = true;
    }


    void LoseGame()
    {
        _isGameActive = false;
        countdownText.text = "¡Perdiste!";
        lightColorChanger.ChangeColorToRed(); 
        StartCoroutine(RestartAfterDelay());
    }
    
    void TimeOutGame()
    {
        _isGameActive = false;
        countdownText.text = "¡Tiempo agotado!";
        lightColorChanger.ChangeColorToRed(); 
        StartCoroutine(RestartAfterDelay());
        
        foreach (var button in numberButtons)
        {
            button.EnableButton(false);
        }
    }

    void WinGame()
    {
        lightColorChanger.ChangeColorToGreen();
        _isGameActive = false;
        countdownText.text = "¡Ganaste!";
        FindObjectOfType<GameManagerServer>()?.SendComplateTask(3);
        
        foreach (var button in numberButtons)
        {
            button.EnableButton(false);
        }
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        StartGame();
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }


}
