using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Start Menu component
public class TitleMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
