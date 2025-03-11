using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource[] walkSounds; 
    public AudioSource[] waterSounds; 
    private int currentWalkSoundIndex = 0;
    private int currentWaterSoundIndex = 0;

    public LayerMask platformLayer;
    public LayerMask waterLayer;

    private Rigidbody2D rb;
    private bool isMoving;
    private bool isOnPlatform;
    private bool isInWater;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        isMoving = (moveX != 0 || moveY != 0);

        isOnPlatform = IsTouchingLayer(platformLayer);
        isInWater = IsTouchingLayer(waterLayer);

        if (isMoving)
        {
            if (isOnPlatform && !walkSounds[currentWalkSoundIndex].isPlaying)
            {
                PlayNextWalkSound();
            }
            else if (isInWater && !waterSounds[currentWaterSoundIndex].isPlaying)
            {
                PlayNextWaterSound();
            }
        }
        else
        {
            StopAllSounds();
        }
    }

    bool IsTouchingLayer(LayerMask layer)
    {
        return rb.IsTouchingLayers(layer);
    }

    void PlayNextWalkSound()
    {
        walkSounds[currentWalkSoundIndex].Play();
        currentWalkSoundIndex = (currentWalkSoundIndex + 1) % walkSounds.Length;
    }

    void PlayNextWaterSound()
    {
        waterSounds[currentWaterSoundIndex].Play();
        currentWaterSoundIndex = (currentWaterSoundIndex + 1) % waterSounds.Length;
    }

    void StopAllSounds()
    {
        foreach (var sound in walkSounds)
        {
            sound.Stop();
        }
        foreach (var sound in waterSounds)
        {
            sound.Stop();
        }
    }
}