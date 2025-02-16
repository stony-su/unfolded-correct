using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource walkSound; // Drag and drop your audio file here

    private Rigidbody2D rb;
    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check if arrow keys are pressed
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Determine if the player is moving
        isMoving = (moveX != 0);

        // Play or stop the sound accordingly
        if (isMoving && !walkSound.isPlaying)
        {
            walkSound.Play();
        }
        else if (!isMoving)
        {
            walkSound.Stop();
        }
    }
}