using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking;
using UnityEngine.XR.Interaction.Toolkit;



[RequireComponent(typeof(XRGrabInteractable))]
public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string keyId = "key_1";
    private XRGrabInteractable grab;
    private bool yaRecolectada = false;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (yaRecolectada) return;
        yaRecolectada = true;

        Debug.Log($"✅ LLAVE AGARRADA: {keyId}");
        AudioManager.instance.Play("pickup_key");

        // Oculta visual y colisión
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        grab.colliders.Clear(); // prevenir agarres adicionales

        SendKeyCollectedMessage();

        // Forzar liberación antes de destruir
        grab.interactionManager.SelectExit(args.interactorObject, grab);

        StartCoroutine(DestruirDespues());
    }

    private IEnumerator DestruirDespues()
    {
        yield return null; // espera 1 frame
        Debug.Log("🗑️ Llave destruida correctamente");
        Destroy(gameObject);
    }

    private void SendKeyCollectedMessage()
    {
        var msg = new NetworkMessage
        {
            type = "event",
            payload = new Payload
            {
                action = "collect_key",
                target = keyId
            }
        };

        SocketServer.Instance.SendMessageToClient(msg);
        Debug.Log($"📨 Enviado al Companion: {keyId}");
    }

    private void OnDestroy()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrab);
    }
}