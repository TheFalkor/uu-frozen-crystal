using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public GameObject pauseScreen;

    public PlayerDeath playerDeath;
    public Slider slider;
    private OrbitCamera cam;

    public static bool isPaused = false;

    void Start()
    {
        pauseScreen.SetActive(false);
        cam = GetComponent<OrbitCamera>();
        slider.value = cam.Sensitivity;
    }

    
    void Update()
    {
        if(Input.GetKeyUp(Config.KEY_PAUSE_GAME))
        {
            TogglePauseGame();
        }
    }

    
    public void ResumeGame()
    {
        TogglePauseGame();
    }

    public void Respawn()
    {
        playerDeath.Respawn();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void TogglePauseGame()
    {
        isPaused = !isPaused;
        Cursor.visible = isPaused;
        if(isPaused)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
        pauseScreen.SetActive(isPaused);
    }

    public void SetSensitivity(float value)
    {
        cam.Sensitivity = value;
    }
}
