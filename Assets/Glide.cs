using UnityEngine;
using UnityEngine.UI;

public class SquirrelGlideController : MonoBehaviour
{
    public float glideSpeed = 5f;
    public float glideStaminaCostPerSecond = 20f;
    public float maxStamina = 100f;
    public float minStamina = 0f;
    public float gravityMultiplier = 0.5f;

    private float currentStamina;
    private bool isGliding = false;
    public bool isTransformed = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public Image staminaBarForeground;

    public Color transformedColor = new Color(1f, 1f, 0f); 
    public Color normalColor = Color.white; 
    public Vector3 transformedScale = new Vector3(1f, 0.5f, 1f); 
    public Vector3 normalScale = new Vector3(1f, 1f, 1f); 

    public float staminaRegenRate = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentStamina = maxStamina;
        UpdateStaminaBar();
    }

    void Update()
    {
        HandleInput();
        HandleGlide();
        UpdateStamina();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleSquirrelForm();
        }
    }

    private void ToggleSquirrelForm()
    {
        if (isTransformed)
        {
            isTransformed = false;
            StopGliding();
        }
        else if (currentStamina > minStamina) 
        {
            isTransformed = true;
            StartGliding();
        }
    }

    private void StartGliding()
    {
        if (isTransformed && currentStamina > minStamina)
        {
            isGliding = true;
            spriteRenderer.color = transformedColor;
            rb.gravityScale = gravityMultiplier;
        }
    }

    private void HandleGlide()
    {
        if (isGliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -glideSpeed));

            currentStamina -= glideStaminaCostPerSecond * Time.deltaTime;

            if (currentStamina <= minStamina)
            {
                StopGliding();
            }
        }
    }

    private void StopGliding()
    {
        isGliding = false;
        isTransformed = false;
        spriteRenderer.color = normalColor;
        rb.gravityScale = 1;
    }

    private void UpdateStamina()
    {
        if (!isGliding && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, minStamina, maxStamina);

        UpdateStaminaBar();
    }

    private void UpdateStaminaBar()
    {
        if (staminaBarForeground != null)
        {
            float fillAmount = currentStamina / maxStamina;
            staminaBarForeground.fillAmount = fillAmount;
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            currentStamina = Mathf.Clamp(currentStamina, minStamina, maxStamina);
            UpdateStaminaBar();
            return true;
        }
        return false;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }
}
