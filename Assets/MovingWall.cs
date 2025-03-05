using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(Rigidbody2D))]
public class DirectionalMovingSquare : MonoBehaviour
{
    [Header("Detection Parameters")]
    public float upDetectionLength = 1f;
    [Range(0, 1)] public float upDetectionWidth = 1f;
    public float downDetectionLength = 1f;
    [Range(0, 1)] public float downDetectionWidth = 1f;
    public float leftDetectionLength = 1f;
    [Range(0, 1)] public float leftDetectionWidth = 1f;
    public float rightDetectionLength = 1f;
    [Range(0, 1)] public float rightDetectionWidth = 1f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public string playerTag = "Player";

    private PolygonCollider2D polygonCollider;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    public bool isMoving;

    [Header("Detection Control")]
    public bool canDetectPlayer = true;
    

    private void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Disable gravity
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            CheckForPlayer();
        }
        else
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    public void ResetMovement()
    {
        isMoving = false;   
        rb.linearVelocity = Vector2.zero;
    }

    private void CheckForPlayer()
    {
        if (!canDetectPlayer) return;

        Vector2 size = polygonCollider.bounds.size;
        float width = size.x;
        float height = size.y;

        // Check Up
        Vector2 upBoxSize = new Vector2(width * upDetectionWidth, upDetectionLength);
        Vector2 upBoxCenter = (Vector2)transform.position + new Vector2(0, height/2 + upDetectionLength/2);
        if (CheckDetectionBox(upBoxCenter, upBoxSize))
        {
            moveDirection = Vector2.up;
            isMoving = true;
            return;
        }

        // Check Down
        Vector2 downBoxSize = new Vector2(width * downDetectionWidth, downDetectionLength);
        Vector2 downBoxCenter = (Vector2)transform.position - new Vector2(0, height/2 + downDetectionLength/2);
        if (CheckDetectionBox(downBoxCenter, downBoxSize))
        {
            moveDirection = Vector2.down;
            isMoving = true;
            return;
        }

        // Check Left
        Vector2 leftBoxSize = new Vector2(leftDetectionLength, height * leftDetectionWidth);
        Vector2 leftBoxCenter = (Vector2)transform.position - new Vector2(width/2 + leftDetectionLength/2, 0);
        if (CheckDetectionBox(leftBoxCenter, leftBoxSize))
        {
            moveDirection = Vector2.left;
            isMoving = true;
            return;
        }

        // Check Right
        Vector2 rightBoxSize = new Vector2(rightDetectionLength, height * rightDetectionWidth);
        Vector2 rightBoxCenter = (Vector2)transform.position + new Vector2(width/2 + rightDetectionLength/2, 0);
        if (CheckDetectionBox(rightBoxCenter, rightBoxSize))
        {
            moveDirection = Vector2.right;
            isMoving = true;
        }   
    }

    private bool CheckDetectionBox(Vector2 center, Vector2 size)
    {
        Collider2D hit = Physics2D.OverlapBox(center, size, 0);
        return hit != null && hit.CompareTag(playerTag);
    }

    private void OnDrawGizmosSelected()
    {
        if (polygonCollider == null) return;

        Vector2 size = polygonCollider.bounds.size;
        float width = size.x;
        float height = size.y;

        Gizmos.color = Color.green;

        // Draw Up Detection
        Gizmos.DrawWireCube(transform.position + new Vector3(0, height/2 + upDetectionLength/2, 0),
            new Vector3(width * upDetectionWidth, upDetectionLength, 1));

        // Draw Down Detection
        Gizmos.DrawWireCube(transform.position - new Vector3(0, height/2 + downDetectionLength/2, 0),
            new Vector3(width * downDetectionWidth, downDetectionLength, 1));

        // Draw Left Detection
        Gizmos.DrawWireCube(transform.position - new Vector3(width/2 + leftDetectionLength/2, 0, 0),
            new Vector3(leftDetectionLength, height * leftDetectionWidth, 1));

        // Draw Right Detection
        Gizmos.DrawWireCube(transform.position + new Vector3(width/2 + rightDetectionLength/2, 0, 0),
            new Vector3(rightDetectionLength, height * rightDetectionWidth, 1));
    }
}