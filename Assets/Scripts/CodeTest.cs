using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeTest : MonoBehaviour
{
    public TextMeshProUGUI[] codeSlotsText;
    public Image[] codeSlotsImage;
    public Image successIndicator;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = new Color32(0x35, 0x9E, 0xB2, 0xFF); // #359EB2

    private string[] possibleLetters = { "A", "B", "Z", "X" };
    private string[] generatedCode;
    public string generatedCodeString { get; private set; }
    private int currentIndex = 0;

    public GameManagerServer gameManagerServer;

    void Start()
    {
        // GenerateCode();
    }

    public void GenerateCode()
    {
        generatedCode = new string[codeSlotsText.Length];
        currentIndex = 0;

        for (int i = 0; i < codeSlotsText.Length; i++)
        {
            string randomLetter = possibleLetters[Random.Range(0, possibleLetters.Length)];
            generatedCode[i] = randomLetter;
            codeSlotsText[i].text = randomLetter;
        }

        generatedCodeString = string.Join("", generatedCode);
        Debug.Log("Code: " + generatedCodeString);
        gameManagerServer.SendTextCode(generatedCodeString);
    }

    public void AddLetter(string input)
    {
        if (currentIndex >= generatedCode.Length) return;

        if (input == generatedCode[currentIndex])
        {
            OnCorrectInput();
        }
        else
        {
            OnWrongInput();
        }
    }

    private void OnCorrectInput()
    {
        codeSlotsImage[currentIndex].color = correctColor;
        currentIndex++;
        gameManagerServer.SendIndexCode(1,currentIndex-1);

        if (currentIndex == generatedCode.Length)
        {
            OnCodeCompleted();
            gameManagerServer.SendIndexCode(3,0);
        }
    }

    private void OnWrongInput()
    {
        gameManagerServer.SendIndexCode(2,0);
        for (int i = 0; i < codeSlotsImage.Length; i++)
        {
            codeSlotsImage[i].color = wrongColor;
        }

        Invoke(nameof(ResetImagesAndGenerateNewCode), 1.0f);
    }

    private void OnCodeCompleted()
    {
        Debug.Log("¡Código completado correctamente!");
        if (successIndicator != null)
            successIndicator.color = Color.green;
    }

    private void ResetImagesAndGenerateNewCode()
    {
        // Reiniciar colores aquí
        for (int i = 0; i < codeSlotsImage.Length; i++)
        {
            codeSlotsImage[i].color = defaultColor;
        }

        GenerateCode();
    }
}
