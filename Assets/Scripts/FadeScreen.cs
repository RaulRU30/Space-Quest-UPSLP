using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = true;
    public float fadeDuration = 2;
    public Color fadeColor = Color.black;
    public AnimationCurve fadeCurve;
    public string colorPropertyName = "_Color";
    private Renderer rend;
    private Material fadeMat;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        
        if(fadeOnStart)
            FadeIn();

    }

    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }
    
    public void FadeIn()
    {
        Fade(1f, 0f);
    }
    
    public void FadeOut()
    {
        Fade(0f, 1f);
    }

    public IEnumerator FadeRoutine(float alphaIn,float alphaOut)
    {
        float timer = 0f;

        while (timer <= fadeDuration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, fadeCurve.Evaluate(timer / fadeDuration));
            rend.material.SetColor(colorPropertyName, newColor);

            timer += Time.deltaTime;
            yield return null;
        }

        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor(colorPropertyName, newColor2);


    }
}
