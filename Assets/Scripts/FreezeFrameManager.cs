using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreezeFrameManager : MonoBehaviour
{
    public Camera playerCamera;
    public Camera freezeFrameCamera;
    public GameObject enemyToFocus;
    public Canvas enemyInfoCanvas;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyDescriptionText;

    private float freezeDuration = 3.0f;

    public void TriggerFreezeFrame(string enemyName, string enemyDescription)
    {
        StartCoroutine(FreezeFrameSequence(enemyName, enemyDescription));
    }

    private IEnumerator FreezeFrameSequence(string name, string description)
    {
        // Pause the game
        Time.timeScale = 0.0f;

        // Switch to the freeze frame camera
        playerCamera.gameObject.SetActive(false);
        freezeFrameCamera.gameObject.SetActive(true);

        // Display UI with enemy info
        enemyInfoCanvas.gameObject.SetActive(true);
        enemyNameText.text = name;
        enemyDescriptionText.text = description;

        // Wait for the specified duration using unscaled time
        yield return new WaitForSecondsRealtime(freezeDuration);

        // Unfreeze the game and switch back
        enemyInfoCanvas.gameObject.SetActive(false);
        freezeFrameCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        Time.timeScale = 1.0f;
    }
}