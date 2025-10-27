using UnityEngine;
using System.Collections; // Needed for Coroutines

public class PistolController : MonoBehaviour
{
    // === PUBLIC REFERENCES ===
    [Header("Model and Visuals")]
    public GameObject pistolModel;
    public GameObject muzzleFlash;

    [Header("Recoil/Shake Settings")]
    public Transform cameraTransform;
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.1f;

    [Header("Weapon Recoil Rotation")]
    public float recoilAngle = 10f;     // The maximum angle the pistol kicks up (e.g., 10 degrees)
    public float recoilTime = 0.08f;    // Time for the kick UP
    public float recoverTime = 0.2f;    // Time for the gun to return DOWN

    [Header("External Controllers")]
    public PlayerInventory playerInventory;

    // === STATE VARIABLES ===
    [HideInInspector] public bool isPistolEquipped = false;
    private Vector3 originalCameraPosition;

    [Header("Firing Rate")]
    public float fireRate = 0.5f; // Time in seconds between shots (e.g., 0.5s)

    // Private state variables for the cooldown
    private float nextTimeToFire = 0f;
    private bool canFire = true; // Initial state: ready to fire

    void Start()
    {
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }
        // Ensure pistol starts holstered
        if (pistolModel != null)
        {
            pistolModel.SetActive(false);
        }
    }

    // Called when '3' is pressed.
    public void TogglePistol()
    {
        isPistolEquipped = !isPistolEquipped;

        if (pistolModel != null)
        {
            pistolModel.SetActive(isPistolEquipped);
        }

        Debug.Log("Pistol Toggled. State: " + (isPistolEquipped ? "EQUIPPED" : "HOLSTERED"));
    }

    // Called when Left Mouse Button is pressed AND the pistol is equipped.
    // Called when Left Mouse Button is pressed AND the pistol is equipped.
    public void FirePistol()
    {
        // 1. FIRE RATE CHECK: Is enough time passed since the last shot?
        if (Time.time < nextTimeToFire)
        {
            return; // Gun is on cooldown, exit the method
        }

        // 2. AMMO CHECK (The existing logic)
        if (playerInventory != null && playerInventory.totalAmmo > 0)
        {
            // --- SHOT IS CONFIRMED ---

            // 3. SET THE NEXT FIRE TIME
            // The gun cannot fire again until the current time + fireRate
            nextTimeToFire = Time.time + fireRate;

            // 4. Consume one bullet and update the UI
            playerInventory.totalAmmo--;
            playerInventory.UpdateInventoryUI();

            // 5. Start visual effects (Muzzle Flash, Recoil, Camera Shake)
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(true);
            }
            StartCoroutine(ShakeCamera());
            StopCoroutine(nameof(RecoilKick));
            StartCoroutine(RecoilKick());
            Invoke(nameof(HideMuzzleFlash), 0.05f);

            Debug.Log($"Pistol Fired! Ammo remaining: {playerInventory.totalAmmo}");
        }
        else
        {
            Debug.Log("Out of ammo! Gun clicks empty.");
            // Allow the "empty click" sound/animation to play even if on cooldown, 
            // but the gun won't fire.
        }
    }

    private void HideMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }

    private IEnumerator ShakeCamera()
    {
        if (cameraTransform == null) yield break; // Safety check

        float elapsed = 0.0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cameraTransform.localPosition = originalCameraPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalCameraPosition;
    }

    private IEnumerator RecoilKick()
    {
        if (pistolModel == null) yield break; // Safety check

        Quaternion originalRotation = pistolModel.transform.localRotation;
        Quaternion kickRotation = originalRotation * Quaternion.Euler(recoilAngle, 0, 0);

        float t = 0;

        // === 1. KICK UP (Fast) ===
        while (t < recoilTime)
        {
            t += Time.deltaTime;
            // Interpolate the rotation from the current rotation towards the max kick rotation
            pistolModel.transform.localRotation = Quaternion.Lerp(originalRotation, kickRotation, t / recoilTime);
            yield return null;
        }

        // Snap the rotation to the max kick position (to ensure it reaches the peak)
        pistolModel.transform.localRotation = kickRotation; 

        t = 0;

        // === 2. RECOVER DOWN (Slower) ===
        while (t < recoverTime)
        {
            t += Time.deltaTime;
            // Interpolate the rotation from the max kick rotation back to the original rotation
            pistolModel.transform.localRotation = Quaternion.Lerp(kickRotation, originalRotation, t / recoverTime);
            yield return null;
        }
        
        // Final snap back to ensure it ends exactly at the starting rotation
        pistolModel.transform.localRotation = originalRotation;
    }


}