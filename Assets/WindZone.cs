using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindCurrent : MonoBehaviour
{
    public float windForce = 20f;
    public Vector2 windDirection = Vector2.up;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SquirrelGlideController player = other.GetComponent<SquirrelGlideController>();
            if (player != null && player.isTransformed)
            {
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Apply wind force
                    rb.AddForce(windDirection.normalized * windForce, ForceMode2D.Force);
                }
            }
        }
    }
}