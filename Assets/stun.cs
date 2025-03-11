using UnityEngine;
using System.Collections;

public class stun : MonoBehaviour
{
    public float stunDuration = 2f;
    public float knockbackForce = 5f; // Add knockback force variable
    private Rigidbody2D rb;
    private bool isStunned = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; 
    }

    public void TakeDamage(int damage)
    {
        if (!isStunned)
        {
            StartCoroutine(Stun());
            ApplyKnockback(); // Apply knockback when taking damage
        }
    }

    private IEnumerator Stun()
    {
        isStunned = true;
        rb.gravityScale = 1; 

        yield return new WaitForSeconds(stunDuration);

        rb.gravityScale = 0; 
        isStunned = false;
    }

    private void ApplyKnockback()
    {
        Vector2 knockbackDirection = (transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }
}