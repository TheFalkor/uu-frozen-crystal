using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public GameObject pauseScreen;

    public static bool isPaused = false;
    
    void Start()
    {
        pauseScreen.SetActive(false);   
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
        Debug.Log("PauseManager :: Respawn script doesn't exist anymore.");
        //Player.isbecomedying("death", true, health = 0, playerdeathenum.DIED);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void TogglePauseGame()
    {
        isPaused = !isPaused;
        Cursor.visible = isPaused;
        pauseScreen.SetActive(isPaused);
    }
}
