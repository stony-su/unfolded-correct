using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;
    public AudioClip attackSFX; // Add this line
    private Animator animator;
    private AudioSource audioSource; // Add this line

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Add this line
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        animator.SetTrigger("Attack");
        audioSource.PlayOneShot(attackSFX); // Add this line
        yield return new WaitForSeconds(0.25f);
        DetectAttack();
    }

    void DetectAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            PlatformerEnemy platformerEnemy = enemy.GetComponent<PlatformerEnemy>();

            if (platformerEnemy != null)
            {
                platformerEnemy.Die();
            }

            DeathHandler deathHandler = enemy.GetComponent<DeathHandler>();
           
            if (deathHandler != null)
            {
                deathHandler.Die();
            }

            BossHealth bossHealth = enemy.GetComponent<BossHealth>();

            if (bossHealth != null)
            {
                bossHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}