using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Set initial color to light purple
            spriteRenderer.color = new Color(0.8f, 0.7f, 1f); 
        }
        else
        {
            Debug.LogError("SpriteRenderer component missing on Checkpoint GameObject.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RespawnManager respawnManager = other.GetComponent<RespawnManager>();
        if (respawnManager != null && !isActivated)
        {
            respawnManager.respawnPoint = transform.position;
            isActivated = true;

            // Change color to light green
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.6f, 1f, 0.6f);
            }
        }
    }
}