using UnityEngine;
using UnityEngine.UI; // Needed for RawImage component

public class PlayerHealth : MonoBehaviour
{
    // === Public References ===
    public RawImage healthPulseImage; // Assign the RawImage UI here

    // === Health Variables ===
    private const int MAX_HEALTH = 6;
    private int currentHealth = MAX_HEALTH; // Start at full health (6)

    // === Color Constants ===
    private readonly Color COLOR_FINE = Color.green;    // Health 5-6
    private readonly Color COLOR_MID = Color.yellow;   // Health 3-4
    private readonly Color COLOR_DANGER = Color.red;   // Health 1-2
    private readonly Color COLOR_DEAD = Color.black;   // Health 0 (or clear)

    void Start()
    {
        // Ensure the UI starts at the correct color
        UpdateHealthUI();
    }

    void Update()
    {
        // === DEBUG/TESTING DAMAGE INPUT ===
        // Press 'H' to deal 1 hit of damage for testing purposes.
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }

    }

    // Public method for taking damage
    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, MAX_HEALTH);
        Debug.Log($"Damage taken. Current Health: {currentHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Public method for using a health pack
    public void Heal(int amount)
    {
        // Health pack restores 2 'hits' (1 health level)
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MAX_HEALTH);
        Debug.Log($"Healed. Current Health: {currentHealth}");

        UpdateHealthUI();
    }

    // Logic to determine color based on health level and update the RawImage
    private void UpdateHealthUI()
    {
        Color targetColor = COLOR_DEAD; // Default to dead/black

        if (currentHealth >= 5) // 5, 6
        {
            targetColor = COLOR_FINE;
        }
        else if (currentHealth >= 3) // 3, 4
        {
            targetColor = COLOR_MID;
        }
        else if (currentHealth >= 1) // 1, 2
        {
            targetColor = COLOR_DANGER;
        }

        // Assign the determined color to the UI RawImage
        healthPulseImage.color = targetColor;
    }

    // Placeholder for death logic
    private void Die()
    {
        Debug.Log("Player has died!");
        // Add game over screen, scene reload, etc. here
    }
}