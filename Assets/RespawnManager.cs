using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    public Vector3 respawnPoint;
    public float fallThreshold = 0f;
    public AudioClip deathSFX;

    private List<RespawnableItem> respawnableItems = new List<RespawnableItem>();
    private Animator animator;
    private bool isRespawning = false;
    private PlayerMovement playerMovement; 
    private AudioSource audioSource; 

    void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); 
        audioSource = GetComponent<AudioSource>(); 
    }

    void Start()
    {
        respawnPoint = transform.position;
        Respawn();
    }

    void Update()
    {
        if (!isRespawning && transform.position.y < fallThreshold)
        {
            StartCoroutine(HandleRespawn());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isRespawning && other.CompareTag("RedBlock"))
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
        playerMovement.enabled = false; 
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; 
        DisableAllAnimations();
        TriggerDeathAnimation();
        PlayDeathSFX(); 
        yield return new WaitForSeconds(1.8f);
        Respawn();
        playerMovement.enabled = true;
        isRespawning = false;
    }

    public void Respawn()
    {
        transform.position = new Vector3(respawnPoint.x, respawnPoint.y-5f, transform.position.z);        

        foreach (var item in respawnableItems)
        {
            if (item != null)
            {
                item.ResetItem();
            }
        }
    }

    private void TriggerDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    private void DisableAllAnimations()
    {
        if (animator != null)
        {
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(parameter.name, false);
                }
                else if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(parameter.name);
                }
            }
        }

        animator.SetTrigger("Death");
    }

    private void PlayDeathSFX() 
    {
         audioSource.PlayOneShot(deathSFX);
        
    }
}