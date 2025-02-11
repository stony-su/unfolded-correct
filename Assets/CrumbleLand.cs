using System.Collections;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float respawnTime = 2f;

    private bool isActive = true;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    private void Start()
    {
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ShakeAndCrumble());
        }
    }

    private IEnumerator ShakeAndCrumble()
    {
        isActive = false;
        float elapsed = 0f;

        // Shake effect
        while (elapsed < shakeDuration)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeMagnitude,
                Random.Range(-1f, 1f) * shakeMagnitude,
                0f
            );
            
            transform.position = originalPosition + shakeOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset position and disable platform
        transform.position = originalPosition;
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;

        // Wait for respawn
        yield return new WaitForSeconds(respawnTime);

        // Enable platform
        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        isActive = true;
    }
}