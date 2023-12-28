using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSystem : MonoBehaviour
{
    public int StartSceneIndex = 1;
    public int SettingsSceneIndex = 2;

    public void StartGame()
    {
        SceneManager.LoadScene(StartSceneIndex);
    }
    public void LoadGame()
    {
    }
    public void Settings()
    {
        SceneManager.LoadScene(SettingsSceneIndex);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
