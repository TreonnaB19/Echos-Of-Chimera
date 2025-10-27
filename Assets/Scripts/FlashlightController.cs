using UnityEngine;
using TMPro; // Needed for the UI text component

public class FlashlightController : MonoBehaviour
{
    // === Public References ===
    public Light flashlight;
    public TextMeshProUGUI percentageUI;
    public float drainRate = 0.05f;      // Power drained per second (e.g., 5% per second)

    // === Private Variables ===
    private bool isFlashlightOn = false;
    private float currentPower = 100f;    // Power is measured in percent (0 to 100)

    void Update()
    {
        // 1. Toggle Input
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }

        // 2. Power Drain Logic (Only drain if the light is on and has power)
        if (isFlashlightOn && currentPower > 0)
        {
            // Drain power based on the drainRate and the time since the last frame
            currentPower -= drainRate * Time.deltaTime;

            // Clamp the power at 0 to prevent negative values
            if (currentPower <= 0)
            {
                currentPower = 0;
                // Turn the flashlight off automatically when power hits zero
                flashlight.enabled = false;
                isFlashlightOn = false;
                Debug.Log("Flashlight power depleted!");
            }

            // 3. Update the UI Text
            UpdateFlashlightUI();
        }
    }

    private void ToggleFlashlight()
    {
        // Only allow the toggle if there is power, or if we are trying to turn it OFF
        if (currentPower > 0 || isFlashlightOn)
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.enabled = isFlashlightOn;

            Debug.Log("Flashlight Toggled. State: " + (isFlashlightOn ? "ON" : "OFF"));
        }
        else if (!isFlashlightOn && currentPower <= 0)
        {
            Debug.Log("Cannot turn on flashlight: No power remaining.");
        }
    }

    private void UpdateFlashlightUI()
    {
        // Rounds the power to the nearest whole number and adds the percent symbol
        percentageUI.text = Mathf.RoundToInt(currentPower) + "%";
    }

    // === PUBLIC METHOD FOR BATTERY USAGE (Called by PlayerPickup.cs) ===
    public void Recharge(float percentage)
    {
        // Add the rechargeable percentage (e.g., 25)
        currentPower += percentage;

        // Clamp the power at 100 to prevent overcharging
        if (currentPower > 100f)
        {
            currentPower = 100f;
        }

        // Ensure the UI is updated after recharging
        UpdateFlashlightUI();
        Debug.Log($"Flashlight recharged by {percentage}%. Current Power: {currentPower:F1}%");

        
    }
}