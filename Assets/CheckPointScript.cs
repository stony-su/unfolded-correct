using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {

            RespawnManager.Instance.respawnPoint = transform.position;
            isActivated = true;
        }
    }
}