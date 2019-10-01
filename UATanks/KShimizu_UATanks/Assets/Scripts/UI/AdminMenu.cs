using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminMenu : MonoBehaviour
{
    public void ResetScores()
    {
        GameManager.instance.highScores.Clear();
    }
    public void OnReset()
    {
        ResetScores();
        ResetPlayerPrefs();
        GameManager.instance.InitializeValues();
    }
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    public void Resume()
    {
        GameManager.instance.gameState = "active";
    }
    public void Menu()
    {
        GameManager.instance.gameState = "menu";
    }
    public void Quit()
    {
        GameManager.instance.gameState = "quit";
    }
}
