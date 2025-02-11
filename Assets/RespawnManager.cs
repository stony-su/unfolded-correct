using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Vector3 respawnPoint; // This is updated by the checkpoint
    public float fallThreshold = -10f; // Adjust as needed

    void Start()
    {
        // Set initial spawn point to the player's starting position
        respawnPoint = transform.position;
        Respawn();
    }

    void Update()
    {
        // If the player falls below a certain threshold, respawn them
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Example: if the player hits a red block (spike), respawn them
        if (other.CompareTag("RedBlock"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = respawnPoint;
    }
}