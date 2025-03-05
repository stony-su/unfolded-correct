using UnityEngine;

[RequireComponent(typeof(DirectionalMovingSquare), typeof(Rigidbody2D))]
public class MovingWallDistanceLimiter : MonoBehaviour
{
    [Tooltip("Maximum distance the wall can move before stopping")]
    public float maxDistance = 5f;

    private DirectionalMovingSquare movingSquare;
    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool wasMoving;

    private void Awake()
    {
        movingSquare = GetComponent<DirectionalMovingSquare>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!movingSquare || !rb) return;

        bool isCurrentlyMoving = movingSquare.isMoving;

        if (isCurrentlyMoving && !wasMoving)
        {
            startPosition = rb.position;
        }

        if (isCurrentlyMoving)
        {
            float distanceMoved = Vector2.Distance(rb.position, startPosition);
            
            if (distanceMoved >= maxDistance)
            {
                movingSquare.ResetMovement();
                movingSquare.canDetectPlayer = false; // Add this line
            }
        }

        wasMoving = isCurrentlyMoving;
    }
}