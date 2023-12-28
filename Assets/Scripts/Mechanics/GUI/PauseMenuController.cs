using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GUISystem GUISystem;
    public int MainMenuSceneIndex = 0;

    public void CloseMenu()
    {
        GUISystem.TogglePause();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(MainMenuSceneIndex);
    }
    
}
