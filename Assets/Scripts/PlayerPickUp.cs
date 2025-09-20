using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public TextMeshProUGUI pickupPrompt;

    private void Update()
    {
        // Create a ray from the center of the camera
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Check if the ray hits an object within pickupRange
        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // If the object has the "Pickup" tag...
            if (hit.collider.CompareTag("Pickup"))
            {
                // ...enable the prompt
                pickupPrompt.enabled = true;

                // Check for the 'E' key press to pick up the item
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Call the pickup method
                    PickUpItem(hit.collider.gameObject);
                }
            }
            else // This 'else' block handles when the ray hits something, but it's not a "Pickup" item
            {
                pickupPrompt.enabled = false;
            }
        }
        else // This 'else' block handles when the ray hits nothing at all
        {
            pickupPrompt.enabled = false;
        }
    }

    private void PickUpItem(GameObject item)
    {
        // Make the item disappear
        item.SetActive(false);
        Debug.Log("Picked up " + item.name);

        // Hide the prompt
        pickupPrompt.enabled = false;
    }
}