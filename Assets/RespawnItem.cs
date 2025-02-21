using UnityEngine;

public class RespawnableItem : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool originalActiveState;
    private Rigidbody2D rb;
    private DirectionalMovingSquare movingSquare; // Declare the variable

    void Start()
    {
        // Save initial position and active state
        originalPosition = transform.position;
        originalActiveState = gameObject.activeSelf;

        // Get Rigidbody2D if it exists
        rb = GetComponent<Rigidbody2D>();

        // Get the DirectionalMovingSquare component if it exists
        movingSquare = GetComponent<DirectionalMovingSquare>();

        // Register this item with the RespawnManager
        RespawnManager.Instance.RegisterItem(this);
    }

    public void ResetItem()
    {
        // Reset position and active state
        transform.position = originalPosition;
        gameObject.SetActive(originalActiveState);

        // Reset velocity if Rigidbody2D exists
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Reset movement if DirectionalMovingSquare component exists
        if (movingSquare != null)
        {
            movingSquare.ResetMovement();
        }

        // Re-enable colliders if needed
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    void OnDestroy()
    {
        // Unregister when destroyed to avoid null references
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.UnregisterItem(this);
        }
    }
}