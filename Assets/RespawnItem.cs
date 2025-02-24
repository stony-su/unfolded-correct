using UnityEngine;
using Bundos.MovingPlatforms; 

public class RespawnableItem : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool originalActiveState;
    private Rigidbody2D rb;
    private DirectionalMovingSquare movingSquare; // Declare the variable
    private PlatformController platformController;

    void Start()
    {
        originalPosition = transform.position;
        originalActiveState = gameObject.activeSelf;

        rb = GetComponent<Rigidbody2D>();
        movingSquare = GetComponent<DirectionalMovingSquare>();
        platformController = GetComponent<PlatformController>(); // Get component

        RespawnManager.Instance.RegisterItem(this);
    }

    public void ResetItem()
    {
        transform.position = originalPosition;
        gameObject.SetActive(originalActiveState);

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (movingSquare != null)
            movingSquare.ResetMovement();

        if (platformController != null) // Reset platform
            platformController.ResetPlatform();

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;
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