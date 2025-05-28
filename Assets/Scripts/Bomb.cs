using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public float explosionDelay = 6f;

    private bool hasLanded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return; // Avoid multiple triggers
        hasLanded = true;
        Debug.Log("Miamorpreciosa");
        // Freeze the bomb's movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Disable physics so it stays still
        }

        if (explosionDelay > 0)
            Invoke(nameof(Explode), explosionDelay);
        else
            Explode();
    }

    private void Explode()
    {
        Debug.Log("💥 Bomb exploded!");

    if (explosionEffect != null)
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

    if (explosionSound != null)
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

    // Detectar objetos cercanos
    float explosionRadius = 3f;
    Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

    foreach (Collider hit in hits)
    {
        if (hit.CompareTag("Player"))
        {
            Debug.Log("🔥 Player is within explosion radius!");
            PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(50f); // Puedes ajustar el daño
            }
        }
    }

    Destroy(gameObject);
    }
}

