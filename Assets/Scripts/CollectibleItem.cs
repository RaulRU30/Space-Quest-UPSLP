using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("🎁 Collected item!");

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Notify the GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.CollectItem();

        Destroy(gameObject);
    }
}
