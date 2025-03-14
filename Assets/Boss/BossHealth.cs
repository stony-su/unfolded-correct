using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;
    private Slider healthSlider;
    private Animator animator;
    public Transform jumpTargetIndicator;
    private bool hasTriggeredHalfHealthAnimation = false; 
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider = GameObject.FindGameObjectWithTag("BossHealthBar").GetComponent<Slider>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();

        if (currentHealth <= maxHealth / 2 && !hasTriggeredHalfHealthAnimation) 
        {
            TriggerHalfHealthAnimation();
            hasTriggeredHalfHealthAnimation = true;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        healthSlider.value = currentHealth;
    }

    void Die()
    {
        animator.SetTrigger("Die");
        StartCoroutine(DieCoroutine());
        jumpTargetIndicator.gameObject.SetActive(false);
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(1.35f); 
        Destroy(healthSlider.gameObject); 
        Destroy(gameObject);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        hasTriggeredHalfHealthAnimation = false; 
    }

    private void TriggerHalfHealthAnimation() 
    {
        animator.SetTrigger("HalfHealth");
    }
}