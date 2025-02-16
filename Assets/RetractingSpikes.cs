using UnityEngine;

public class RetractingSpikes : MonoBehaviour
{
    public enum SpikeDirection { Up, Down, Left, Right }

    [Header("Direction Settings")]
    public SpikeDirection direction = SpikeDirection.Up;
    public float maxScale = 2f;

    [Header("Timing Settings")]
    public float extendDuration = 0.8f;
    public float retractDuration = 0.5f;
    public float extendedWait = 1.2f;
    public float retractedWait = 0.7f;
    public float initialDelay = 0f; // New delay parameter
    private Vector3 baseScale;
    private Vector3 targetScale;
    private Vector2[] baseColliderPoints;
    private PolygonCollider2D spikeCollider;



    void Start()
    {
        spikeCollider = GetComponent<PolygonCollider2D>();
        baseScale = transform.localScale;

        SetTargetScale();
        StartCoroutine(SpikeCycle());
    }

    private void SetTargetScale()
    {
        switch (direction)
        {
            case SpikeDirection.Up:
                targetScale = new Vector3(baseScale.x, baseScale.y * maxScale, baseScale.z);
                break;
            case SpikeDirection.Down:
                targetScale = new Vector3(baseScale.x, baseScale.y * maxScale, baseScale.z);
                break;
            case SpikeDirection.Left:
                targetScale = new Vector3(baseScale.x * maxScale, baseScale.y, baseScale.z);
                break;
            case SpikeDirection.Right:
                targetScale = new Vector3(baseScale.x * maxScale, baseScale.y, baseScale.z);
                break;
        }
    }

    private System.Collections.IEnumerator SpikeCycle()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            // Extend spike
            yield return ScaleSpike(baseScale, targetScale, extendDuration);
            yield return new WaitForSeconds(extendedWait);

            // Retract spike
            yield return ScaleSpike(targetScale, baseScale, retractDuration);
            yield return new WaitForSeconds(retractedWait);
        }
    }

    private System.Collections.IEnumerator ScaleSpike(Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(fromScale, toScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = toScale;
    }

    private Vector2 GetScaledPoint(Vector2 point, Vector3 targetScale)
    {
        switch (direction)
        {
            case SpikeDirection.Up:
                return new Vector2(point.x, point.y * (targetScale.y / baseScale.y));
            case SpikeDirection.Down:
                return new Vector2(point.x, point.y * (targetScale.y / baseScale.y));
            case SpikeDirection.Left:
                return new Vector2(point.x * (targetScale.x / baseScale.x), point.y);
            case SpikeDirection.Right:
                return new Vector2(point.x * (targetScale.x / baseScale.x), point.y);
            default:
                return point;
        }
    }

    // Optional damage implementation
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Implement your damage logic here
            Debug.Log("Player hit spikes!");
        }
    }
}