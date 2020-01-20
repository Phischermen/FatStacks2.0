using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuActions : MonoBehaviour
{
    public string startLevel;
    public string artTestLevel;
    public GameObject settingsMenu;
    //public MusicTrack theme;

    public void StartClicked()
    {
        SceneManager.LoadScene(startLevel);
        //MusicManager.singleton.QueueUpNextTrack(theme);
    }

    public void ArtTestClicked()
    {
        SceneManager.LoadScene(artTestLevel);
        MusicManager.singleton.PlayTrack(null);
    }

    public void QuitClicked()
    {
        Application.Quit();
    }

    public void SettingsClicked()
    {
        gameObject.SetActive(false);
        settingsMenu.SetActive(true);
    }
}
