using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    public Vector3 respawnPoint;
    public float fallThreshold = 0f;
    public AudioClip deathSFX;

    public GameObject player1;
    public GameObject player2;

    private List<RespawnableItem> respawnableItems = new List<RespawnableItem>();
    private bool isRespawning = false;
    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        respawnPoint = transform.position;
        Respawn();
    }

    public void PlayerDied()
    {
        if (!isRespawning)
        {
            StartCoroutine(HandleRespawn());
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

    private IEnumerator HandleRespawn()
    {
        isRespawning = true;

        // Disable movement for both players
        SetPlayerMovementEnabled(false);
        
        // Reset velocities
        ResetPlayerVelocities();

        // Handle animations
        TriggerDeathAnimations();

        PlayDeathSFX();
        yield return new WaitForSeconds(1.8f);

        Respawn();
        
        // Re-enable movement
        SetPlayerMovementEnabled(true);

        isRespawning = false;
    }

    public void Respawn()
    {
        if (player1 != null)
            player1.transform.position = new Vector3(respawnPoint.x, respawnPoint.y, player1.transform.position.z);
        if (player2 != null)
            player2.transform.position = new Vector3(respawnPoint.x, respawnPoint.y, player2.transform.position.z);

        foreach (var item in respawnableItems)
            if (item != null)
                item.ResetItem();
    }

    private void SetPlayerMovementEnabled(bool enabled)
    {
        foreach (var player in new[] { player1, player2 })
        {
            var movement = player?.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = enabled;
        }
    }

    private void ResetPlayerVelocities()
    {
        foreach (var player in new[] { player1, player2 })
        {
            var rb = player?.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    private void TriggerDeathAnimations()
    {
        foreach (var player in new[] { player1, player2 })
        {
            var animator = player?.GetComponent<Animator>();
            if (animator != null)
            {
                DisableAllAnimations(animator);
                animator.SetTrigger("Death");
            }
        }
    }

    private void DisableAllAnimations(Animator animator)
    {
        if (animator == null) return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                animator.SetBool(parameter.name, false);
            else if (parameter.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(parameter.name);
        }
    }

    private void PlayDeathSFX()
    {
        if (audioSource != null && deathSFX != null)
            audioSource.PlayOneShot(deathSFX);
    }
}