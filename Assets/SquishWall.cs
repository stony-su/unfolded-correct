using UnityEngine;
using System.Collections.Generic;

public class SquishWall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckForSquish(collision.collider, collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckForSquish(collision.collider, collision.gameObject);
        }
    }

    void CheckForSquish(Collider2D playerCollider, GameObject player)
    {
        // Check if player is touching any ground objects
        bool isTouchingGround = false;
        List<Collider2D> contacts = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter();

        if (playerCollider.Overlap(filter, contacts) > 0)
        {
            foreach (Collider2D col in contacts)
            {
                if (col != null && col.CompareTag("Walls"))
                {
                    isTouchingGround = true;
                    break;
                }
            }
        }

        if (isTouchingGround)
        {
            RespawnManager respawnManager = player.GetComponent<RespawnManager>();
            if (respawnManager != null)
            {
                respawnManager.Respawn();
            }
        }
    }
}