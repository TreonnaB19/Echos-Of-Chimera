using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public FreezeFrameManager freezeManager;
    public string enemyName = "Cy-Twins";
    public string enemyDescription = "A Two Headed Cyborg Creature.";
    public Transform playerCamera; // Drag the main camera here
    public float detectionRange = 20.0f; // Distance to detect the enemy
    private bool hasBeenTriggered = false;

    private void Update()
    {
        // Only check if the event hasn't been triggered yet
        if (!hasBeenTriggered)
        {
            // Cast a ray from the player's camera forward
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            // Check if the ray hits a collider within the detection range
            if (Physics.Raycast(ray, out hit, detectionRange))
            {
                // Check if the hit object is the enemy
                if (hit.collider.gameObject == this.gameObject)
                {
                    // The player is looking at the enemy, trigger the freeze frame
                    freezeManager.TriggerFreezeFrame(enemyName, enemyDescription);
                    hasBeenTriggered = true; // Prevents re-triggering
                }
            }
        }
    }
}