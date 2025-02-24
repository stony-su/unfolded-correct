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

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        FloatingScissorsEnemy enemy = collision.gameObject.GetComponent<FloatingScissorsEnemy>();
        if (enemy != null)
        {
            Vector2 enemyPosition = collision.transform.position;
            TakeDamage(50, enemyPosition);
        }
    }

    void TakeDamage(int damage, Vector2 enemyPosition)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Apply knockback
        Vector2 knockbackDirection = ((Vector2)transform.position - enemyPosition).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // Flash red
        StartCoroutine(FlashRed());

        // Update health bar
        UpdateHealthBar();

        // Check if health is zero
        if (currentHealth <= 0)
        {
            StartCoroutine(RespawnAfterDelay());
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
}