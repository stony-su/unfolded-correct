using UnityEngine;

public class RetractingSpikes : MonoBehaviour
{
    public enum SpikeDirection { Up, Down, Left, Right }

    [Header("Direction Settings")]
    public SpikeDirection direction = SpikeDirection.Up;

    [Header("Timing Settings")]
    public float extendedWait = 1.2f;
    public float retractedWait = 0.7f;
    public float initialDelay = 0f; 

    private Animator animator;
    private Collider2D spikeCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        spikeCollider = GetComponent<Collider2D>();
        StartCoroutine(SpikeCycle());
    }

    private System.Collections.IEnumerator SpikeCycle()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            animator.SetTrigger("Extend");
            yield return new WaitForSeconds(extendedWait);

            animator.SetTrigger("Retract");
            yield return new WaitForSeconds(retractedWait);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit spikes!");
        }
    }

    public void EnableCollider()
    {
        spikeCollider.enabled = true;
    }

    public void DisableCollider()
    {
        spikeCollider.enabled = false;
    }
}