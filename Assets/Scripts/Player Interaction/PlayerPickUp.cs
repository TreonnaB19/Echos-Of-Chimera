using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class PlayerPickup : MonoBehaviour
{
    // === PUBLIC REFERENCES ===
    [Header("Detection")]
    public float pickupRange = 3f;
    public TextMeshProUGUI pickupPrompt;

    [Header("Controllers")]
    public PlayerInventory playerInventory;
    public PistolController pistolController;
    public PlayerHealth playerHealthController;

    [Header("Scene/Level Settings")]
    // This should be set to "Level_03" for Level 2 exit
    public string nextLevelSceneName = "Level_03";
    public string mainMenuSceneName = "MainMenu"; // New: Scene to load when the game is complete

    [Header("Level 3 Settings")]
    // Set this to TRUE when this script is running in the final level of the build
    public bool isFinalLevel = false;

    [Header("Transition Effects")]
    public CanvasGroup fadeScreenGroup;
    public float fadeDuration = 1.0f;

    private void Start()
    {
        // Automatically fade in when the level starts
        if (fadeScreenGroup != null)
        {
            // SAFETY: Ensure the game object is active so it renders
            fadeScreenGroup.gameObject.SetActive(true);

            // Set alpha to 1 (black) instantly, then fade out
            fadeScreenGroup.alpha = 1f;
            StartCoroutine(FadeInSequence());
        }
    }

    void Update()
    {
        // === 1. RAYCAST SETUP ===
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        bool promptWasSet = false;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            GameObject hitObject = hit.collider.gameObject;

            // --- ITEM PICKUP ---
            if (hitObject.CompareTag("Pickup"))
            {
                pickupPrompt.enabled = true;
                pickupPrompt.text = "E - Pick Up";
                promptWasSet = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    HandleItemPickup(hitObject);
                }
            }

            // --- EXIT DOOR LOGIC (LEVEL 2/3 OBJECTIVE CHECK) ---
            else if (hitObject.CompareTag("ExitDoor"))
            {
                // **CRITICAL CHECK:** Is the evidence objective complete?
                // This checks if evidenceFound is >= 5 (PlayerInventory.MAX_EVIDENCE)
                bool exitIsReady = playerInventory.evidenceFound >= PlayerInventory.MAX_EVIDENCE;

                string levelAction = isFinalLevel ? "Finish Game" : $"Exit Level ({nextLevelSceneName})";

                if (exitIsReady)
                {
                    pickupPrompt.text = $"E - {levelAction}";
                }
                else
                {
                    pickupPrompt.text = "E - Investigate Door";
                }

                pickupPrompt.enabled = true;
                promptWasSet = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Call the handler, passing the boolean result of the check
                    HandleDoorInteraction(exitIsReady);
                }
            }
        }

        if (!promptWasSet)
        {
            pickupPrompt.enabled = false;
        }

        // === 2. INVENTORY INPUTS ===
        if (Input.GetKeyDown(KeyCode.Alpha1)) playerInventory.ConsumeHealthPack();
        if (Input.GetKeyDown(KeyCode.Alpha2)) playerInventory.ConsumeBattery();
        if (Input.GetKeyDown(KeyCode.Alpha3)) pistolController.TogglePistol();
        if (Input.GetButtonDown("Fire1") && pistolController.isPistolEquipped) pistolController.FirePistol();

        // === 3. DEBUG INPUT ===
        if (Input.GetKeyDown(KeyCode.H) && playerHealthController != null) playerHealthController.TakeDamage(1);
    }

    private void HandleItemPickup(GameObject item)
    {
        int pickupStatus = playerInventory.AddItem(item);

        if (pickupStatus == 0)
        {
            item.SetActive(false);
            pickupPrompt.enabled = false;
        }
        else if (pickupStatus == 1)
        {
            playerInventory.ShowStatusMessage("FULL");
        }
    }

    // Checks the exit condition before triggering the transition
    private void HandleDoorInteraction(bool exitIsReady)
    {
        if (exitIsReady)
        {
            if (isFinalLevel)
            {
                // Final Level: End the game sequence
                GameComplete();
            }
            else
            {
                // Standard Level: Transition to the next scene
                StartCoroutine(FadeOutAndLoadLevel());
            }
        }
        else
        {
            // Condition NOT met: Block the exit and show feedback
            playerInventory.ShowStatusMessage("I should find evidence before I continue.");
            Debug.Log("Exit blocked: Evidence collection incomplete.");
        }
    }

    // NEW FUNCTION: Handles the completion of the game (Level 3 exit)
    private void GameComplete()
    {
        Debug.Log("--- GAME COMPLETE --- Final objective met! Loading Main Menu.");

        // This triggers the fade out before loading the main menu scene.
        StartCoroutine(FadeOutAndEndGame());
    }


    // === TRANSITION COROUTINES ===

    private IEnumerator FadeOutAndLoadLevel()
    {
        // Prevent multiple calls
        pickupPrompt.enabled = false;

        if (fadeScreenGroup != null)
        {
            // SAFETY: Ensure it's active
            fadeScreenGroup.gameObject.SetActive(true);

            float t = 0;
            fadeScreenGroup.blocksRaycasts = true; // Block input while fading

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                // Fade Alpha from 0 (Transparent) to 1 (Black)
                fadeScreenGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
        }

        Debug.Log($"Fade Complete. Loading scene: {nextLevelSceneName}");
        SceneManager.LoadScene(nextLevelSceneName);
    }

    private IEnumerator FadeOutAndEndGame()
    {
        // Prevent player input and hide the prompt
        pickupPrompt.enabled = false;

        if (fadeScreenGroup != null)
        {
            // Ensure time scale is back to normal if a previous level froze it
            Time.timeScale = 1;

            fadeScreenGroup.gameObject.SetActive(true);

            float t = 0;
            fadeScreenGroup.blocksRaycasts = true; // Block input while fading

            while (t < fadeDuration * 2) // Fade out slightly longer for finality
            {
                t += Time.deltaTime;
                fadeScreenGroup.alpha = Mathf.Clamp01(t / (fadeDuration * 2));
                yield return null;
            }
        }

        Debug.Log($"Game Finished. Loading Main Menu scene: {mainMenuSceneName}");
        // Load the main menu scene instead of freezing time.
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadeInSequence()
    {
        if (fadeScreenGroup != null)
        {
            // SAFETY: Ensure it's active
            fadeScreenGroup.gameObject.SetActive(true);

            float t = 0;
            fadeScreenGroup.blocksRaycasts = true; // Block input while fading in

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                // Fade Alpha from 1 (Black) to 0 (Transparent)
                fadeScreenGroup.alpha = Mathf.Clamp01(1 - (t / fadeDuration));
                yield return null;
            }

            fadeScreenGroup.blocksRaycasts = false; // Allow input again
        }
    }
}