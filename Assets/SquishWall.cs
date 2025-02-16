using UnityEngine;

public class SquishDeath : MonoBehaviour
{
    // Tag for the player object
    private const string PlayerTag = "Player";

    // Tag for the ground/walls
    private const string WallsTag = "Walls";

    // Minimum distance to consider the player squished
    public float squishDistanceThreshold = 0.1f;

    private void Update()
    {
        CheckForSquish();
    }

    private void CheckForSquish()
    {
        // Get all colliders near the moving wall
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, squishDistanceThreshold);

        bool isPlayerNear = false;
        bool isWallNear = false;

        foreach (Collider collider in nearbyColliders)
        {
            // Check if the collider is the player
            if (collider.CompareTag(PlayerTag))
            {
                isPlayerNear = true;
            }

            // Check if the collider is a wall
            if (collider.CompareTag(WallsTag))
            {
                isWallNear = true;
            }
        }

        // If the player is near and a wall is near, the player is squished
        if (isPlayerNear && isWallNear)
        {
            //RespawnManager.Instance.Respawn();
        }
    }

    // Optional: Visualize the squish detection area in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, squishDistanceThreshold);
    }
}