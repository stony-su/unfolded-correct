using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    public float detectionDepth = 1f;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.1f;
    public float respawnTime = 2f; 
    public LayerMask playerLayer;

    private Vector3 originalPosition;
    private bool isShaking = false;
    private float platformWidth;
    private Collider2D platformCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        originalPosition = transform.position;
        platformCollider = GetComponent<Collider2D>();
        platformWidth = platformCollider.bounds.size.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isShaking) return; 

        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (detectionDepth / 2 + 0.1f);
        Vector2 boxSize = new Vector2(platformWidth, detectionDepth);

        Collider2D player = Physics2D.OverlapBox(boxCenter, boxSize, 0, playerLayer);

        if (player)
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
            transform.position = originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        platformCollider.enabled = false;
        spriteRenderer.enabled = false;

        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);

        platformCollider.enabled = true;
        spriteRenderer.enabled = true;
        isShaking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float width = GetComponent<Collider2D>() ? GetComponent<Collider2D>().bounds.size.x : 1f;
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (detectionDepth / 2 + 0.1f);
        Vector2 boxSize = new Vector2(width, detectionDepth);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}