using UnityEngine;
using System.Collections.Generic;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    public Vector3 respawnPoint;
    public float fallThreshold = -10f;

    private List<RespawnableItem> respawnableItems = new List<RespawnableItem>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        respawnPoint = transform.position;
        Respawn();
    }

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RedBlock"))
        {
            Respawn();
        }
    }

    public void RegisterItem(RespawnableItem item)
    {
        respawnableItems.Add(item);
    }

    public void UnregisterItem(RespawnableItem item)
    {
        respawnableItems.Remove(item);
    }

    public void Respawn()
    {
        // Reset player position
        transform.position = respawnPoint;
        
        // Reset all registered items
        foreach (var item in respawnableItems)
        {
            if (item != null)
            {
                item.ResetItem();
            }
        }
    }
}