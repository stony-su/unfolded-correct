using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FloatingScissorsEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float detectionRadius = 5f;
    public float attackRadius = 2f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Attack Settings")]
    public float telegraphTime = 0.5f;
    public float attackCooldown = 1f;
    public float launchDistance = 3f;
    public float pullBackDistance = 0.3f;

    [Header("References")]
    public LineRenderer attackLine;
    public Animator animator;
    public string attackAnimationName = "Attack";

    private Transform player;
    private NavMeshAgent agent;
    private bool isAttacking = false;
    private Vector2 attackDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        attackLine.enabled = false;
    }

    void Update()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            StartCoroutine(AttackSequence());
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            agent.SetDestination(player.position);
            UpdateRotation(agent.velocity.normalized);
        }
        else
        {
            agent.ResetPath();
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        agent.ResetPath();

        // Determine attack direction
        attackDirection = (player.position - transform.position).normalized;
        UpdateRotation(attackDirection);

        // Pull back
        Vector2 pullBackPosition = (Vector2)transform.position - attackDirection * pullBackDistance;
        yield return MoveToPosition(pullBackPosition, 0.1f);

        // Show telegraph line
        attackLine.enabled = true;
        attackLine.startWidth = 0.05f;  // Make the line thin
        attackLine.endWidth = 0.05f;    

        attackLine.startColor = new Color(1f, 0f, 0f);
        attackLine.endColor = new Color(1f, 0f, 0f);

        // Position the line in front of the enemy
        Vector2 midPoint = (Vector2)transform.position + attackDirection * (launchDistance / 2f);
        Vector2 startPoint = midPoint - attackDirection * (launchDistance / 2f);
        Vector2 endPoint = midPoint + attackDirection * (launchDistance / 2f);

        attackLine.SetPosition(0, startPoint);
        attackLine.SetPosition(1, endPoint);

        yield return new WaitForSeconds(telegraphTime);
        attackLine.enabled = false;

        // Play attack animation
        if (animator != null)
        {
            animator.Play(attackAnimationName);
        }

        // Launch attack
        Vector2 launchTarget = (Vector2)transform.position + attackDirection * launchDistance;
        yield return MoveToPosition(launchTarget, 0.1f);

        // Cooldown
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator MoveToPosition(Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    void UpdateRotation(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}