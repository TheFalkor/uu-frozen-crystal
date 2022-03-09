using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool LOAD_ALL_SCENES = false;

    void Start()
    {
        if (!LOAD_ALL_SCENES)
            return;

        string name = "Scenes/Level_";
        for(int i = 1; i <= 7; i++)
            SceneManager.LoadScene(name + i, LoadSceneMode.Additive);
        print("test");
    }
}
