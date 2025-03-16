using UnityEngine;
using System.Collections;

public class Volley : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float destroyDelay = 0.5f;
    private Vector2 targetPosition;
    private Animator animator;
    private bool hasLanded = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(Vector2 targetPosition, float heightOffset)
    {
        this.targetPosition = targetPosition;
        transform.position = new Vector2(targetPosition.x, targetPosition.y + heightOffset); // Apply height offset
        animator.SetBool("inTheAir", true);
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        while (!hasLanded)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            if (hit.collider != null && hit.collider.CompareTag("Platform"))
            {
                hasLanded = true;
                OnLand();
            }
            yield return null;
        }
    }

    void OnLand()
    {
        animator.SetBool("inTheAir", false);
        Destroy(gameObject, destroyDelay);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(playerHealth.maxHealth * 0.2f, transform.position);
            }
        }
        if (other.CompareTag("Platform"))
        {
            hasLanded = true;
            OnLand();
        }
    }
}