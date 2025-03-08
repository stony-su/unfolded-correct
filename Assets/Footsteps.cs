using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource[] walkSounds; // Drag and drop your audio files here
    private int currentSoundIndex = 0;

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
        if (isMoving && !walkSounds[currentSoundIndex].isPlaying)
        {
            PlayNextSound();
        }
        else if (!isMoving)
        {
            StopAllSounds();
        }
    }

    void PlayNextSound()
    {
        walkSounds[currentSoundIndex].Play();
        currentSoundIndex = (currentSoundIndex + 1) % walkSounds.Length;
    }

    void StopAllSounds()
    {
        foreach (var sound in walkSounds)
        {
            sound.Stop();
        }
    }
}