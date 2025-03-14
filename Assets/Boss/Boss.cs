using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Boss : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;

    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float jumpDamage = 100f;
    public float jumpMinTime = 1f;
    public float jumpMaxTime = 3f;
    public Transform jumpTargetIndicator;
    public float knockbackForce = 10f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isJumping = false;
    private BossHealth bossHealth;
    private int attackCounter = 0; 
    private bool isInCooldown = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossHealth = GetComponent<BossHealth>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        JumpAttack();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange && !isJumping && !isInCooldown)
            {
                Attack();
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
        else
        {
            Idle();
        }
    }

    void MoveTowardsPlayer()
    {
        if (isJumping) return; 

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * knockbackForce, rb.linearVelocity.y); 
        animator.SetBool("isWalking", true);

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Idle()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);
    }

    void Attack()
    {
        if (attackCounter >= 1 || isInCooldown) 
        {
            StartCoroutine(IdleAfterAttacks());
            return;
        }
        StartCoroutine(JumpAttack());
        attackCounter++;
    }

    IEnumerator JumpAttack()
    {
        isJumping = true;
        animator.SetTrigger("Jump");
        float jumpTime = Random.Range(jumpMinTime, jumpMaxTime);
        jumpTargetIndicator.gameObject.SetActive(true);

        Collider2D bossCollider = GetComponent<Collider2D>();
        bossCollider.enabled = false;

        float elapsedTime = 0f;
        while (elapsedTime < jumpTime - 0.5f)
        {
            jumpTargetIndicator.position = player.position;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        jumpTargetIndicator.gameObject.SetActive(false);
        Vector3 playerPositionBeforeJump = player.position; 
        yield return new WaitForSeconds(0.5f);
        transform.position = new Vector3(playerPositionBeforeJump.x, playerPositionBeforeJump.y + 30f, playerPositionBeforeJump.z); 
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        animator.SetBool("isFalling", true);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        bossCollider.enabled = true;

        yield return new WaitForSeconds(1.2f);
        animator.SetBool("isFalling", false);
        animator.SetTrigger("Land");
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (bossCollider.bounds.Intersects(playerCollider.bounds))
        {
            player.GetComponent<PlayerHealth>().TakeDamage(jumpDamage, transform.position);
        }

        isJumping = false;
    }

    IEnumerator IdleAfterAttacks()
    {
        isInCooldown = true; 
        Idle();
        yield return new WaitForSeconds(2f);
        attackCounter = 0; 
        isInCooldown = false; 
    }

    public void TakeDamage(int damage)
    {
        bossHealth.TakeDamage(damage);
    }
    void UpdateHealthBar()
    {
        float healthRatio = (float)currentHealth / maxHealth;
    }

    public void CancelJumpAttack()
    {
        StopAllCoroutines(); 
        isJumping = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isFalling", false);
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Land");
        jumpTargetIndicator.gameObject.SetActive(false);
        Vector3 playerPositionBeforeJump = new Vector3(-13.88f, -24.7f, 0f); 
    }
}