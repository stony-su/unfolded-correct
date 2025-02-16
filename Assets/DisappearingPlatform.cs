using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    public float detectionDepth = 1f; // How far below the platform the detection area extends
    public float shakeDuration = 1f; // How long the platform shakes before disappearing
    public float shakeMagnitude = 0.1f; // How much the platform shakes
    public LayerMask playerLayer; // Assign in Inspector to detect only the player

    private Vector3 originalPosition;
    private bool isShaking = false;
    private float platformWidth;

    void Start()
    {
        originalPosition = transform.position;
        platformWidth = GetComponent<Collider2D>().bounds.size.x; // Get platform width
    }

    void Update()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (detectionDepth / 2 + 0.1f); // Position below platform
        Vector2 boxSize = new Vector2(platformWidth, detectionDepth); // Auto-sized width, adjustable depth

        // Check if the player is inside the detection box
        Collider2D player = Physics2D.OverlapBox(boxCenter, boxSize, 0, playerLayer);

        if (player && !isShaking)
        {
            StartCoroutine(ShakeAndDisappear());
        }
    }

    IEnumerator ShakeAndDisappear()
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Apply shaking effect
            transform.position = originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset position and disable platform
        transform.position = originalPosition;
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Red gizmo for visibility
        float width = GetComponent<Collider2D>() ? GetComponent<Collider2D>().bounds.size.x : 1f; // Default width if no collider
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (detectionDepth / 2 + 0.1f);
        Vector2 boxSize = new Vector2(width, detectionDepth);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
