using UnityEngine;

public class PlayerBuoyancy : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    [SerializeField] private float targetSubmerged = 0.3f; // Target submerged percentage for equilibrium
    [SerializeField] private float waterDrag = 3f;          // Drag when in water
    [SerializeField] private float waterAngularDrag = 1f;   // Angular drag when in water

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private float originalDrag;
    private float originalAngularDrag;
    private float waterSurfaceY;
    private bool isInWater = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        originalDrag = rb.linearDamping;
        originalAngularDrag = rb.angularDamping;
    }

    private void FixedUpdate()
    {
        if (isInWater)
        {
            float playerBottom = playerCollider.bounds.min.y;
            float submergedDepth = waterSurfaceY - playerBottom;
            float playerHeight = playerCollider.bounds.size.y;
            float submergedPercentage = Mathf.Clamp(submergedDepth / playerHeight, 0f, 1f);

            float gravity = Mathf.Abs(Physics2D.gravity.y);
            float buoyantForce = (submergedPercentage / targetSubmerged) * rb.mass * gravity;
            rb.AddForce(Vector2.up * buoyantForce);

            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;
        }
        else
        {
            rb.linearDamping = originalDrag;
            rb.angularDamping = originalAngularDrag;
        }
    }

    #region Water Triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            waterSurfaceY = other.bounds.max.y; 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
        }
    }
    }
    #endregion