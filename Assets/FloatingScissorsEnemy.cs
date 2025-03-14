using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FloatingScissorsEnemy : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float attackRadius = 2f;

    [Header("Attack Settings")]
    public float launchDistance = 5f;
    public float telegraphTime = 0.5f;
    public float launchSpeed = 10f;
    public float pullBackSpeed = 3f;

    [Header("References")]
    public LineRenderer attackLine;

    [Header("Cooldown Settings")]
    public float attackCooldown = 0f; 

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isOnCooldown = false;
    private Vector2 attackDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        ConfigureNavMeshAgent();
        attackLine.enabled = false;
    }

    void ConfigureNavMeshAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = attackRadius * 0.8f;
    }

    void Update()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius && !isOnCooldown)
        {
            StartCoroutine(AttackSequence());
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        animator.SetBool("IsFlying", true);
        RotateTowards(player.position - transform.position);
    }

    void StopChasing()
    {
        agent.SetDestination(transform.position);
        animator.SetBool("IsFlying", false);
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        agent.enabled = false;
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsFlying", false);

        attackDirection = (player.position - transform.position).normalized;

        yield return StartCoroutine(PullBackMovement());

        yield return StartCoroutine(ShowAttackTelegraph());

        yield return StartCoroutine(PerformAttack());

        agent.enabled = true;
        isAttacking = false;
        animator.SetBool("IsAttacking", false);

        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;

        if (Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            animator.SetBool("IsFlying", true);
        }
    }

    IEnumerator PullBackMovement()
    {
        float pullBackDuration = 0.3f;
        float timer = 0f;
        Vector2 pullBackDirection = -attackDirection;

        while (timer < pullBackDuration)
        {
            rb.linearVelocity = pullBackDirection * pullBackSpeed;
            RotateTowards(pullBackDirection);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ShowAttackTelegraph()
    {
        attackLine.startWidth = 0.05f; 
        attackLine.endWidth = 0.05f;
        attackLine.material.color = Color.red; 

        attackLine.enabled = true;
        attackLine.SetPosition(0, transform.position);
        attackLine.SetPosition(1, (Vector2)transform.position + attackDirection * launchDistance);

        yield return new WaitForSeconds(telegraphTime);

        attackLine.enabled = false;
    }

    IEnumerator PerformAttack()
    {
        rb.linearVelocity = attackDirection * launchSpeed;
        RotateTowards(attackDirection);

        float attackDuration = launchDistance / launchSpeed;
        yield return new WaitForSeconds(attackDuration);

        rb.linearVelocity = Vector2.zero;
    }

    void RotateTowards(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}