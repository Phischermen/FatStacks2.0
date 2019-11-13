using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuActions : MonoBehaviour
{
    public GameObject optionsMenu;
    public string mainMenuScene;

    public void Resume()
    {
        Player.singleton.TogglePause();
        Player.singleton.GetComponentInChildren<MouseLook>().SetSensitivity(PlayerPrefs.GetFloat("Sensitivity"));
        Player.singleton.GetComponentInChildren<Camera>().fieldOfView = PlayerPrefs.GetFloat("FOV");
    }

    public void Options()
    {
        gameObject.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
