using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    void OnTriggerEnter2D(Collider2D other)
    {
        RespawnManager respawnManager = other.GetComponent<RespawnManager>();
        if (respawnManager != null && !isActivated)
        {
            respawnManager.respawnPoint = transform.position;
            isActivated = true;
        }
    }
}