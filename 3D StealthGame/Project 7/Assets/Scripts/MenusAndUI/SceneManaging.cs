using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneManaging : Singleton<SceneManaging>
{
    [SerializeField]
    private GameObject goThisMenu;

    public void LoadingScene(int a_iSceneID)
    {

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadSceneAsync(a_iSceneID);
        Time.timeScale = 1.0f;
        
        if(a_iSceneID == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ChangeMenu(GameObject a_goToShow)
    {
        goThisMenu.SetActive(false);
        a_goToShow.SetActive(true);
    }

    public void ReloadScene(int a_iSceneID)
    {
        Time.timeScale = 1.0f;

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(a_iSceneID);
        if(a_iSceneID == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        goThisMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}