using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    // === PUBLIC REFERENCES ===
    [Header("Detection")]
    public float pickupRange = 3f;
    public TextMeshProUGUI pickupPrompt;

    [Header("Controllers")]
    public PlayerInventory playerInventory;
    public PistolController pistolController;
    public PlayerHealth playerHealthController; // For testing damage 'H'

    void Update()
    {
        // === 1. RAYCAST AND PICKUP LOGIC ===
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Raycast successful and hit a "Pickup" tag
        if (Physics.Raycast(ray, out hit, pickupRange) && hit.collider.CompareTag("Pickup"))
        {
            pickupPrompt.enabled = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                // DELEGATE THE PICKUP ACTION to the Inventory script
                if (playerInventory.AddItem(hit.collider.gameObject))
                {
                    hit.collider.gameObject.SetActive(false);
                    pickupPrompt.enabled = false;
                }
            }
        }
        else
        {
            pickupPrompt.enabled = false;
        }

        // === 2. INVENTORY USAGE INPUTS (Delegation) ===

        // Consume Health Pack ('1')
        if (Input.GetKeyDown(KeyCode.Alpha1))
            playerInventory.ConsumeHealthPack();

        // Consume Battery ('2')
        if (Input.GetKeyDown(KeyCode.Alpha2))
            playerInventory.ConsumeBattery();

        // Toggle Pistol ('3')
        if (Input.GetKeyDown(KeyCode.Alpha3))
            pistolController.TogglePistol();

        // Fire Pistol (Left Click)
        if (Input.GetButtonDown("Fire1") && pistolController.isPistolEquipped)
            pistolController.FirePistol();

        // === 3. TESTING DAMAGE INPUT ('H') ===
        if (Input.GetKeyDown(KeyCode.H) && playerHealthController != null)
            playerHealthController.TakeDamage(1);
    }
}