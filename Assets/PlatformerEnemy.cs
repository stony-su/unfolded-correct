using UnityEngine;
using System.Collections;

public class PlatformerEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    private int currentWaypointIndex = 0;
    private bool isMovingForward = true;

    [Header("Combat Settings")]
    public float attackRadius = 1.5f;
    public int attackDamage = 25;
    public float attackCooldown = 1f;
    private bool canAttack = true;

    [Header("References")]
    public Animator animator;
    public Transform attackPoint;
    private Transform player;
    private PlayerHealth playerHealth;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    void Update()
    {
        if (isDead) return;

        CheckPlayerInRange();
        HandleMovement();
        UpdateDirection();
    }

    void HandleMovement()
    {
        if (waypoints.Length < 2) return;
        if (!canAttack) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = target.position - transform.position;

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); 
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            if (isMovingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    isMovingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    isMovingForward = true;
                }
            }
        }

        animator.SetBool("Run", true);
    }

    void CheckPlayerInRange()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRadius && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        
        yield return new WaitForSeconds(0.5f);

        if (Vector2.Distance(transform.position, player.position) <= attackRadius)
        {
            playerHealth.TakeDamage(attackDamage, transform.position);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void UpdateDirection()
    {
        if (waypoints.Length < 2) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = target.position - transform.position;
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); 
        }
    }

    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        StartCoroutine(DestroyAfterDelay(1.5f)); 
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Die();
        }
    }
}