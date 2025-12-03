using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerInventory : MonoBehaviour
{
    // === INVENTORY DATA ===
    [Header("Inventory Data")]
    public int healthPacks = 0;
    public int batteries = 0;
    public int totalAmmo = 0;

    // NEW DATA FOR LEVEL 2
    public int evidenceFound = 0;

    private const int MAX_HEALTH_PACKS = 3;
    private const int MAX_BATTERIES = 3;
    public const int MAX_EVIDENCE = 5; // The required amount

    // === UI REFERENCES ===
    [Header("UI References")]
    public TextMeshProUGUI healthCountUI;
    public TextMeshProUGUI batteryCountUI;
    public TextMeshProUGUI ammoCountUI;
    public TextMeshProUGUI statusTextUI;

    // NEW UI REFERENCE FOR OBJECTIVES
    [Header("Objective UI")]
    public TextMeshProUGUI objectiveTextUI;

    // === EXTERNAL CONTROLLER REFERENCES ===
    [Header("External Controllers")]
    public PlayerHealth playerHealthController;
    public FlashlightController flashlightController;

    private const int HEAL_AMOUNT = 2;        // Health pack heals 2 'hits'
    private const float RECHARGE_AMOUNT = 25f; // Battery recharges 25%

    void Start()
    {
        UpdateInventoryUI();
        UpdateObjectiveUI(); // New call to update objective text
    }

    // Called by PlayerPickup.cs when 'E' is pressed.
    // Update the return type: 0 = Success, 1 = Full
    public int AddItem(GameObject item)
    {
        PickupItem pickup = item.GetComponent<PickupItem>();
        if (pickup == null)
        {
            Debug.LogError("Pickup item is missing the 'PickupItem' script!");
            return -1;
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
            // NEW LOGIC FOR EVIDENCE
            case ItemType.Evidence:
                if (evidenceFound < MAX_EVIDENCE)
                {
                    evidenceFound++;
                    wasPickedUp = true;
                }
                break;
        }

        if (wasPickedUp)
        {
            Debug.Log($"Picked up {pickup.type} for +{pickup.amount}.");
            UpdateInventoryUI();
            UpdateObjectiveUI(); // Update objectives after pickup
            return 0;
        }
        else
        {
            // Only show "FULL" for capped items (Health & Battery)
            if (pickup.type != ItemType.Ammo)
            {
                return 1; // Inventory Full
            }
            return 0; // Ammo doesn't show full message
        }
    }

    // NEW METHOD: Updates the objective text based on current evidence count
    public void UpdateObjectiveUI()
    {
        if (objectiveTextUI)
        {
            string exitStatus = (evidenceFound >= MAX_EVIDENCE)
                ? "<color=green>1. Find the Exit</color>"
                : "1. Find the Exit";

            objectiveTextUI.text =
                $"{exitStatus}\n" +
                $"2. Find all Evidence {evidenceFound}/{MAX_EVIDENCE}";
        }
    }

    // Method to briefly display a status message
    public void ShowStatusMessage(string message)
    {
        if (statusTextUI != null)
        {
            // Make sure the object is active if it was previously disabled
            statusTextUI.gameObject.SetActive(true);

            statusTextUI.text = message;
            statusTextUI.enabled = true;

            CancelInvoke(nameof(HideStatusMessage));
            Invoke(nameof(HideStatusMessage), 1.5f);
        }
    }

    public void HideStatusMessage()
    {
        if (statusTextUI != null)
        {
            statusTextUI.enabled = false;
        }
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
            ShowStatusMessage("No Health Packs");
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
            ShowStatusMessage("No Batteries");
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