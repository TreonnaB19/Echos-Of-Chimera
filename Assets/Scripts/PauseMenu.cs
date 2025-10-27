using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public MonoBehaviour CharacterControllerMovement;
    public GameObject gameUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        gameUI.SetActive(true);
        CharacterControllerMovement.enabled = true;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked; // Lock cursor for gameplay
        Cursor.visible = false; // Hide cursor during gameplay
    }

    void Pause()
    {
        gameUI.SetActive(false);
        CharacterControllerMovement.enabled = false;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Make cursor visible
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Loading Menu...");
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}

