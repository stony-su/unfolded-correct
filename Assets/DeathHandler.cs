using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DeathHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string deathTrigger = "Die";
    [SerializeField] private float destroyDelay = 1f;

    private Animator animator;
    private bool isDead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger(deathTrigger);

        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component != this && component != animator)
            {
                component.enabled = false;
            }
        }
        transform.rotation = Quaternion.identity;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        Destroy(gameObject, destroyDelay);
    }
}