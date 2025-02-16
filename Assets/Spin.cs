using UnityEngine;

public class SpinningObject2D : MonoBehaviour
{
    // Public variables for spin speed and direction
    public float spinSpeed = 100f; // Speed of the spin in degrees per second
    public enum SpinDirection { Clockwise, CounterClockwise }
    public SpinDirection spinDirection = SpinDirection.Clockwise;

    void Update()
    {
        // Determine the direction multiplier
        float directionMultiplier = (spinDirection == SpinDirection.Clockwise) ? -1f : 1f;

        // Rotate the object around the Z-axis
        transform.Rotate(0, 0, directionMultiplier * spinSpeed * Time.deltaTime);
    }
}