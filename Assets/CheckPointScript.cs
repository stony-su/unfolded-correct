using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color32(21, 120, 140, 255);
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

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color32(255, 105, 115, 255);
            }
        }
    }
}