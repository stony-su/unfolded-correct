using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Attacks")]
    [SerializeField] private Collider2D stompHitbox;
    [SerializeField] private Collider2D spitHitbox;
    [SerializeField] private float stompCooldown = 3f;
    [SerializeField] private float jumpCooldown = 5f;
    [SerializeField] private float spitCooldown = 4f;
    [SerializeField] private Vector2 jumpAttackSize = new Vector2(3f, 0.5f);
    [SerializeField] private GameObject telegraphPrefab;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private GameObject currentTelegraph;
    private bool isAttacking;
    private float nextStompTime;
    private float nextJumpTime;
    private float nextSpitTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;

        DisableAllAttackHitboxes();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isAttacking)
        {
            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer > attackRange)
                {
                    MoveTowardsPlayer();
                }
                else
                {
                    agent.ResetPath();
                    ChooseAttack();
                }
            }
            else
            {
                Idle();
            }
        }

        UpdateFlipping();
    }

    private void MoveTowardsPlayer()
    {
        agent.SetDestination(player.position);
        animator.SetBool("IsWalking", true);
        Debug.Log("Moving towards player");
    }

    private void Idle()
    {
        agent.ResetPath();
        animator.SetBool("IsWalking", false);
    }

    private void UpdateFlipping()
    {
        if (agent.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (agent.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void ChooseAttack()
    {
        if (Time.time >= nextStompTime)
        {
            StartCoroutine(StompAttack());
            nextStompTime = Time.time + stompCooldown;
            Debug.Log("Stomp attack");
        }
        else if (Time.time >= nextJumpTime)
        {
            StartCoroutine(JumpAttack());
            nextJumpTime = Time.time + jumpCooldown;
            Debug.Log("Jump attack");
        }
        else if (Time.time >= nextSpitTime)
        {
            StartCoroutine(SpitAttack());
            Debug.Log("Spit attack");
            nextSpitTime = Time.time + spitCooldown;
        }
    }

    private IEnumerator StompAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Stomp");
        agent.ResetPath();

        // Wait for animation event to activate hitbox
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    private IEnumerator JumpAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Jump");
        agent.ResetPath();

        // Jump phase
        Vector2 originalPosition = transform.position;
        float airTime = Random.Range(1f, 3f);
        Vector2 targetPosition = player.position;

        // Show telegraph
        currentTelegraph = Instantiate(telegraphPrefab, targetPosition, Quaternion.identity);
        SpriteRenderer telegraphRenderer = currentTelegraph.GetComponent<SpriteRenderer>();
        
        // Telegraph animation
        float flashTime = 0.5f;
        Color originalColor = telegraphRenderer.color;
        
        for (float t = 0; t < airTime; t += Time.deltaTime)
        {
            float progress = t / airTime;
            telegraphRenderer.size = new Vector2(jumpAttackSize.x * progress, jumpAttackSize.y);
            yield return null;
        }

        // Flash before impact
        telegraphRenderer.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        Destroy(currentTelegraph);

        // Land (damage handled by separate ground impact collider)
        transform.position = targetPosition;
        isAttacking = false;
    }

    private IEnumerator SpitAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Spit");
        agent.ResetPath();

        // Wait for animation event to activate spit
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    // Animation Events
    public void EnableStompHitbox() => stompHitbox.enabled = true;
    public void DisableStompHitbox() => stompHitbox.enabled = false;
    
    public void EnableSpitHitbox() => spitHitbox.enabled = true;
    public void DisableSpitHitbox() => spitHitbox.enabled = false;

    private void DisableAllAttackHitboxes()
    {
        stompHitbox.enabled = false;
        spitHitbox.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}