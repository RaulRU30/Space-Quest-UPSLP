using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CodeTest : MonoBehaviour
{
    public CodeGenerator codeGenerator;
    public Image[] codeSlots;
    public Image successIndicator;
    private int currentIndex = 0;
    private Color defaultColor = new Color32(0x35, 0x9E, 0xB2, 0xFF); // #359EB2
    public GameManagerServer gameManagerServer;

    public void AddLetter(string letter)
    {
        string generatedCode = codeGenerator.GetGeneratedCode();

        if (currentIndex >= generatedCode.Length)
            return;

        string expectedLetter = generatedCode[currentIndex].ToString();

        Debug.Log($"Letra presionada: {letter}, esperada: {expectedLetter}");

        if (letter == expectedLetter)
        {
            if (codeSlots != null && codeSlots.Length > currentIndex)
            {
                codeSlots[currentIndex].color = Color.green;
                gameManagerServer.SendIndexCode(1);   
            }

            currentIndex++;

            if (currentIndex == generatedCode.Length)
            {
                Debug.Log("Código correcto");
                //gameManagerServer.SendIndexCode(3);

                if (successIndicator != null)
                    successIndicator.color = Color.green;
            }
        }
        else
        {
            Debug.Log("Letra incorrecta, reiniciado");
            //gameManagerServer.SendIndexCode(1);
            StartCoroutine(ResetWithRedFlash());
        }
    }

    private IEnumerator ResetWithRedFlash()
    {
        foreach (var slot in codeSlots)
        {
            if (slot != null)
                slot.color = Color.red;
        }

        yield return new WaitForSeconds(1f);

        ResetCode();
    }

    public void ResetCode()
    {
        currentIndex = 0;

        foreach (var slot in codeSlots)
        {
            if (slot != null)
                slot.color = defaultColor;
        }
        codeGenerator.GenerateCode();
    }
}
