using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DirectionalMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float maxTravelDistance = 5f;
    [Range(0, 360), SerializeField] float movementAngle = 0f;

    [Header("Debug")]
    [SerializeField] bool showGizmos = true;

    private Rigidbody2D rb;
    private Vector2 initialPosition;
    private Vector2 movementDirection;
    private int currentDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = rb.position;
        UpdateMovementDirection();
    }

    void FixedUpdate()
    {
        UpdateMovementDirection();
        MoveObject();
    }

    void UpdateMovementDirection()
    {
        float angleRad = movementAngle * Mathf.Deg2Rad;
        movementDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
    }

    void MoveObject()
    {
        Vector2 currentPosition = rb.position;
        float displacement = Vector2.Dot(currentPosition - initialPosition, movementDirection);

        if (Mathf.Abs(displacement) >= maxTravelDistance)
        {
            // Clamp position to the max travel distance
            Vector2 clampedPosition = initialPosition + movementDirection * Mathf.Sign(displacement) * maxTravelDistance;
            rb.position = clampedPosition;
            currentDirection *= -1;
        }

        rb.linearVelocity = movementDirection * moveSpeed * currentDirection;
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos || !Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Vector2 startPoint = initialPosition - movementDirection * maxTravelDistance;
        Vector2 endPoint = initialPosition + movementDirection * maxTravelDistance;
        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawWireSphere(startPoint, 0.2f);
        Gizmos.DrawWireSphere(endPoint, 0.2f);
    }
}