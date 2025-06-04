using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSpriteSwap : MonoBehaviour
{
    [SerializeField] private XRBaseInteractable interactable;
    [SerializeField] private Image targetGraphic;

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private Sprite pressedSprite;
    
    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }
    
    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Hover entered on " + gameObject.name);
        targetGraphic.sprite = highlightedSprite;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        Debug.Log("Hover exited on " + gameObject.name);
        targetGraphic.sprite = normalSprite;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Select entered on " + gameObject.name);
        targetGraphic.sprite = pressedSprite;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("Select exited on " + gameObject.name);
        targetGraphic.sprite = normalSprite;
    }

    private void OnDestroy()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
    }

}
