using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour
{
    // Public Variables
    public Image staminaBarForeground;  // Reference to the foreground image of the stamina bar
    public float maxStamina = 100f;     // Maximum stamina the player can have
    public float currentStamina;        // Current stamina (can be modified during gameplay)
    public float staminaRegenRate = 5f; // How quickly stamina regenerates over time

    private void Start()
    {
        // Initialize the stamina at max value
        currentStamina = maxStamina;
        
        // Initialize stamina bar
        UpdateStaminaBar();
    }

    private void Update()
    {
        // Regenerate stamina over time if it's not full
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure stamina stays within limits
        }

        // Update stamina bar UI
        UpdateStaminaBar();
    }

    // This method will update the stamina bar's foreground fill amount
    private void UpdateStaminaBar()
    {
        // If the staminaBarForeground reference is not null
        if (staminaBarForeground != null)
        {
            // Update the fillAmount of the foreground image based on current stamina percentage
            float fillAmount = currentStamina / maxStamina;
            staminaBarForeground.fillAmount = fillAmount;
        }
    }

    // Method to decrease stamina (e.g., when the player is gliding)
    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure stamina doesn't go below 0
    }

    // Method to increase stamina (e.g., when the player is not transformed)
    public void IncreaseStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure stamina doesn't exceed maxStamina
    }
}
