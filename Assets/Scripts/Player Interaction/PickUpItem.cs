using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemType type;
    public int amount = 1; // Default amount, but we'll override this for ammo

    // Use this to define the properties for this specific item in the Inspector
}