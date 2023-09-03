using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenu : MonoBehaviour
{
    //Sliding Menu
    public GameObject navigationMenu;
    private bool isActive = false;
    public void ExitScene()
    {
        // TODO: replace with go-to start menu in final application
        //Application.Quit();
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowMenu()
    {
        isActive = !isActive;
        navigationMenu.SetActive(isActive);
    }
}
