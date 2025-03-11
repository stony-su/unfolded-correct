using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Transform HealthBarForeground;
    public float knockbackForce = 10f;
    public float flashDuration = 0.1f;
    public float respawnDelay = 0.2f;
    public float invulnerabilityDuration = 0.5f; 

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvulnerable = false; 

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (transform.position.y < RespawnManager.Instance.fallThreshold)
            RespawnManager.Instance.PlayerDied();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RedBlock"))
            RespawnManager.Instance.PlayerDied();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvulnerable) return; 

        FloatingScissorsEnemy enemy = collision.gameObject.GetComponent<FloatingScissorsEnemy>();
        if (enemy != null)
        {
            Vector2 enemyPosition = collision.transform.position;
            TakeDamage(50, enemyPosition);
        }
    }

    public void TakeDamage(int damage, Vector2 enemyPosition)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Vector2 knockbackDirection = ((Vector2)transform.position - enemyPosition).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(FlashRed());

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine()); 
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void UpdateHealthBar()
    {
        float healthRatio = (float)currentHealth / maxHealth;
        HealthBarForeground.localScale = new Vector3(healthRatio, 1f, 1f);
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        RespawnManager.Instance.Respawn();
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
}