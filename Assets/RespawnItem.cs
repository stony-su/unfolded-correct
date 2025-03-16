using UnityEngine;
using Bundos.MovingPlatforms; 

public class RespawnableItem : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool originalActiveState;
    private Rigidbody2D rb;
    private DirectionalMovingSquare movingSquare;
    private PlatformController platformController;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalActiveState = gameObject.activeSelf;

        rb = GetComponent<Rigidbody2D>();
        movingSquare = GetComponent<DirectionalMovingSquare>();
        platformController = GetComponent<PlatformController>();

        RegisterWithManager();
    }

    public void RegisterWithManager()
    {
        RespawnManager.Instance.RegisterItem(this);
    }

    public void ResetItem()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        gameObject.SetActive(originalActiveState);

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

         if (movingSquare != null)
        {
            movingSquare.ResetMovement();
            movingSquare.canDetectPlayer = true; 
        }

        if (platformController != null) 
            platformController.ResetPlatform();

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;
    }

    void OnDestroy()
    {
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.UnregisterItem(this);
        }
    }
}