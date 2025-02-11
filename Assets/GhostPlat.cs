using System.Collections;
using UnityEngine;

public class GhostPlatform : MonoBehaviour
{
    private PlatformEffector2D platformEffector;
    public float resetTime = 0.2f; // Time before re-enabling collision after player drops down

    void Start()
    {
        // Get the PlatformEffector2D component from the current GameObject
        platformEffector = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        HandlePlatformInput();
    }

    private void HandlePlatformInput()
    {
        // Check if the player is pressing the down arrow key
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift))
            {
                DropThroughPlatform();
            }
        }
    }

    private void DropThroughPlatform()
    {
        // Set the effector's rotational offset to allow falling through
        platformEffector.rotationalOffset = 180f;
        StartCoroutine(ResetPlatformCollision());
    }

    private IEnumerator ResetPlatformCollision()
    {
        yield return new WaitForSeconds(resetTime);
        
        // Reset the effector's rotational offset back to the original state
        platformEffector.rotationalOffset = 0f;
    }
}
