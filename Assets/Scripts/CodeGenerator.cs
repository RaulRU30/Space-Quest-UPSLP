using UnityEngine;
using TMPro;

public class CodeGenerator : MonoBehaviour
{
    public TextMeshProUGUI[] codeSlots; // Arreglo de 6 textos
    private string[] possibleLetters = { "A", "B", "Z", "X" };
    private string generatedCode = "";
    public GameManagerServer gameManagerServer;
    void Start()
    {
        GenerateCode();
    }

    public void GenerateCode()
    {
        generatedCode = "";

        for (int i = 0; i < codeSlots.Length; i++)
        {
            string randomChar = possibleLetters[Random.Range(0, possibleLetters.Length)];
            //codeSlots[i].text = randomChar;
            generatedCode += randomChar;
        }
        gameManagerServer.SendTextCode(generatedCode);
        //Debug.Log("Codigo generado: " + generatedCode);
    }

    public string GetGeneratedCode(){
        Debug.Log("Codigo generado CodeGenerator: " + generatedCode);
        return generatedCode;
    }
}
