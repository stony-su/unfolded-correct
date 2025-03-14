using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public enum MovementDirection { Up, Down, Left, Right }
    public MovementDirection direction = MovementDirection.Left;

    public float moveLength = 5f; 
    public float moveTime = 2f;  
    public float waitTime = 1f;  

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime;
    private bool isMoving = true;
    private bool isWaiting = false;

    void Start()
    {
        startPosition = transform.position;
        CalculateTargetPosition();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveObject();
        }
        else if (isWaiting)
        {
            Wait();
        }
    }

    void CalculateTargetPosition()
    {
        switch (direction)
        {
            case MovementDirection.Up:
                targetPosition = startPosition + Vector3.up * moveLength;
                break;
            case MovementDirection.Down:
                targetPosition = startPosition + Vector3.down * moveLength;
                break;
            case MovementDirection.Left:
                targetPosition = startPosition + Vector3.left * moveLength;
                break;
            case MovementDirection.Right:
                targetPosition = startPosition + Vector3.right * moveLength;
                break;
        }
    }

    void MoveObject()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveTime);

        transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        if (t >= 1f)
        {
            isMoving = false;
            isWaiting = true;
            elapsedTime = 0f;
        }
    }

    void Wait()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= waitTime)
        {
            isWaiting = false;
            isMoving = true;
            elapsedTime = 0f;

            Vector3 temp = startPosition;
            startPosition = targetPosition;
            targetPosition = temp;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPosition, targetPosition);
        }
    }
}