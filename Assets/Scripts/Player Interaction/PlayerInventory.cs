using UnityEngine;
using TMPro;
using UnityEngine.UI; // Needed for RawImage

public class PlayerInventory : MonoBehaviour
{
    // === INVENTORY DATA ===
    [Header("Inventory Data")]
    public int healthPacks = 0;
    public int batteries = 0;
    public int totalAmmo = 0;

    private const int MAX_HEALTH_PACKS = 3;
    private const int MAX_BATTERIES = 3;

    // === UI REFERENCES ===
    [Header("Inventory UI References")]
    public TextMeshProUGUI healthCountUI;
    public TextMeshProUGUI batteryCountUI;
    public TextMeshProUGUI ammoCountUI;

    // === EXTERNAL CONTROLLER REFERENCES ===
    [Header("External Controllers")]
    public PlayerHealth playerHealthController;
    public FlashlightController flashlightController;

    private const int HEAL_AMOUNT = 2;       // Health pack heals 2 'hits'
    private const float RECHARGE_AMOUNT = 25f; // Battery recharges 25%

    void Start()
    {
        UpdateInventoryUI(); // Set initial UI values to zero
    }

    // Called by PlayerPickup.cs when 'E' is pressed.
    public bool AddItem(GameObject item)
    {
        PickupItem pickup = item.GetComponent<PickupItem>();
        if (pickup == null)
        {
            Debug.LogError("Pickup item is missing the 'PickupItem' script!");
            return false;
        }

        bool wasPickedUp = false;

        switch (pickup.type)
        {
            case ItemType.HealthPack:
                if (healthPacks < MAX_HEALTH_PACKS)
                {
                    healthPacks += pickup.amount;
                    wasPickedUp = true;
                }
                break;
            case ItemType.Battery:
                if (batteries < MAX_BATTERIES)
                {
                    batteries += pickup.amount;
                    wasPickedUp = true;
                }
                break;
            case ItemType.Ammo:
                totalAmmo += pickup.amount;
                wasPickedUp = true;
                break;
        }

        if (wasPickedUp)
        {
            Debug.Log($"Picked up {pickup.type} for +{pickup.amount}.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log($"Inventory full for {pickup.type}. Cannot pick up.");
        }

        return wasPickedUp;
    }

    // Called when '1' is pressed.
    public void ConsumeHealthPack()
    {
        if (healthPacks > 0)
        {
            if (playerHealthController != null)
            {
                healthPacks--;
                playerHealthController.Heal(HEAL_AMOUNT);
                UpdateInventoryUI();
            }
            else
            {
                Debug.LogError("Player Health Controller reference is missing!");
            }
        }
        else
        {
            Debug.Log("No health packs to use.");
        }
    }

    // Called when '2' is pressed.
    public void ConsumeBattery()
    {
        if (batteries > 0)
        {
            if (flashlightController != null)
            {
                batteries--;
                flashlightController.Recharge(RECHARGE_AMOUNT);
                UpdateInventoryUI();
            }
            else
            {
                Debug.LogError("Flashlight Controller reference is missing!");
            }
        }
        else
        {
            Debug.Log("No batteries to use.");
        }
    }

    // Updates the text display for all inventory items
    public void UpdateInventoryUI()
    {
        if (healthCountUI) healthCountUI.text = healthPacks.ToString();
        if (batteryCountUI) batteryCountUI.text = batteries.ToString();
        if (ammoCountUI) ammoCountUI.text = totalAmmo.ToString();
    }
}