using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float acceleration = 0.3f;
    public float deceleration = 0.3f;
    public float jumpForce = 6f;
    public float jumpHBoost = 0.1f;
    public float liftBoost = 0.3f;
    public float gravity = 8f;
    public float fallSpeedCap = -4f;
    public float earlyJumpBuffer = 4f / 60f;
    public float coyoteTime = 4f / 60f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [SerializeField] private float wallJumpRunForceMultiplier = 0.05f;

    [Header("Dash Settings")]
    private Rigidbody2D rb;
    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private bool airDashAllowed = true;
    [SerializeField] private ParticleSystem dashTrail;
    private Vector2 dashDirection;
    [SerializeField] private int dashEffectCount = 3;
    [SerializeField] private GameObject dashEffectPrefab;
    [SerializeField] private float characterDirection = 1f;
    [SerializeField] private Transform characterSprite;

    [Header("Water Settings")]
    public float swimDownSpeed = 3f;
    public float waterPushSpeed = 2f;
    public float waterMoveSpeedMultiplier = 0.6f;
    public float waterAccelerationMultiplier = 0.6f;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool jumpRequested;
    private bool jumpPerformed;
    private float lastGroundedTime;
    private float jumpPressedTime;
    private Vector2 dashStartPos;
    private bool isInWater = false;
    private float originalGravity;

    [Header("Walljump Settings")]
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 8f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.6f;
    public float wallJumpForce = 8f;
    private float wallJumpPushForce = 75f;
    [SerializeField] private Transform wallCheckright;
    [SerializeField] private Transform wallCheckleft;
    [SerializeField] private LayerMask wallLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        originalGravity = rb.gravityScale;

        if (dashTrail != null)
        {
            dashTrail.Stop();
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            jumpPerformed = false;
        }

        if (!isGrounded && Time.time - jumpPressedTime > earlyJumpBuffer)
        {
            jumpRequested = false;
        }

        if (jumpRequested && !jumpPerformed && (Time.time - lastGroundedTime <= coyoteTime))
        {
            Jump();
            jumpRequested = false;
        }

        if (Input.GetKeyDown(KeyCode.X) && CanDash())
        {
            Vector2 input = GetArrowKeyDirection();
            dashDirection = input == Vector2.zero ? new Vector2(characterDirection, 0) : input;
            dashDirection = dashDirection.normalized;

            StartCoroutine(Dash(dashDirection));
            StartCoroutine(FreezeFrame(0.03f));

            if (dashTrail != null)
            {
                float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
                float xRotation = (angle) % 360f;
                dashTrail.transform.rotation = Quaternion.Euler(xRotation, 270f, 0f);
                dashTrail.Play();
            }

            LeanTween.scale(characterSprite.gameObject, new Vector2(1.4f, 0.4f), 0.1f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => {
                    LeanTween.scale(characterSprite.gameObject, Vector2.one, 0.1f)
                        .setEase(LeanTweenType.easeInQuad);
                });

            dashStartPos = transform.position;
            dashEffectCount = 3;
        }

        WallSlide();
        WallJump();
    }

    #region Wall Jump
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheckright.position, 0.2f, wallLayer) || 
               Physics2D.OverlapCircle(wallCheckleft.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !isGrounded)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        bool isTouchingRightWall = Physics2D.OverlapCircle(wallCheckright.position, 0.2f, wallLayer);
        bool isTouchingLeftWall = Physics2D.OverlapCircle(wallCheckleft.position, 0.2f, wallLayer);

        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            jumpRequested = false;
            
            if (isTouchingRightWall)
            {
                rb.linearVelocity = new Vector2(wallJumpPushForce, wallJumpForce);
            }
            else if (isTouchingLeftWall)
            {
                rb.linearVelocity = new Vector2(-1 * wallJumpPushForce, wallJumpForce);
            }
            
            wallJumpingCounter = 0f;
            Invoke(nameof(StopWallJump), wallJumpingDuration);
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }
    #endregion

    private void FixedUpdate()
    {
        float currentAcceleration = acceleration;
        float currentMoveSpeed = moveSpeed;
        wallJumpPushForce = 75f;
        if (isWallJumping)
        {
            currentAcceleration = 0;
            currentMoveSpeed = currentMoveSpeed * wallJumpRunForceMultiplier;
            currentMoveSpeed = currentMoveSpeed > 0 ? 0.1f : 0f;
            if (moveInput.x != 0)
            {
                wallJumpPushForce = wallJumpPushForce - 30f;
            }
        }

        // Water movement
        if (isInWater)
        {
            currentMoveSpeed *= waterMoveSpeedMultiplier;
            currentAcceleration *= waterAccelerationMultiplier;

            if (moveInput.y < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -swimDownSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, waterPushSpeed);
            }
        }
        else if (!isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                Mathf.Max(rb.linearVelocity.y - gravity * Time.fixedDeltaTime, fallSpeedCap));
        }

        float targetSpeed = moveInput.x * currentMoveSpeed;
        float accelerationRate = Mathf.Abs(targetSpeed) > 0.01f ? currentAcceleration : deceleration;
        rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetSpeed, accelerationRate), rb.linearVelocity.y);

        if (isDashing && dashEffectCount > 0)
        {
            float distance = Vector2.Distance(dashStartPos, transform.position);
            if (distance >= 1.6f) 
            {
                SpawnDashEffect();
                dashStartPos = transform.position;
                dashEffectCount--;
            }
        }
    }

    #region Dash
    private IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        
        rb.linearVelocity = direction * (moveInput.y > 0 ? dashSpeed * 0.5f : 
            (moveInput.x != 0 || moveInput.y != 0) ? dashSpeed * 2.5f: dashSpeed * 5f);

        if (dashTrail != null) dashTrail.Play();

        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;

        if (dashTrail != null && dashTrail.isPlaying) dashTrail.Stop();
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void SpawnDashEffect()
    {
        if (dashEffectPrefab != null)
        {
            GameObject effect = Instantiate(dashEffectPrefab, transform.position, transform.rotation);
            SpriteRenderer effectSprite = effect.GetComponent<SpriteRenderer>();
            if (effectSprite != null)
            {
                effectSprite.sprite = GetComponent<SpriteRenderer>().sprite;
            }
            StartCoroutine(FadeOutEffect(effect, 0.5f));
        }
    }

    private IEnumerator FadeOutEffect(GameObject effect, float fadeDuration)
    {
        SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        float timer = fadeDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = timer / fadeDuration;
            spriteRenderer.color = color;
            yield return null;
        }
        Destroy(effect);
    }

    private IEnumerator FreezeFrame(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private Vector2 GetArrowKeyDirection()
    {
        return new Vector2(
            Keyboard.current.rightArrowKey.isPressed ? 1 : Keyboard.current.leftArrowKey.isPressed ? -1 : 0,
            Keyboard.current.upArrowKey.isPressed ? 1 : Keyboard.current.downArrowKey.isPressed ? -1 : 0
        );
    }

    private bool CanDash()
    {
        return canDash && !isDashing && (isGrounded || airDashAllowed) && !isInWater;
    }

    public void ActivateDashPowerup()
    {
        canDash = true;
    }
    #endregion

    #region Jump
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpRequested = true;
            jumpPressedTime = Time.time;
        }
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void Jump()
    {
        if (isInWater) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + jumpHBoost * moveInput.x, jumpForce);
        jumpPerformed = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    #endregion

    #region Water Triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            rb.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
            rb.gravityScale = originalGravity;
        }
    }
    #endregion
}