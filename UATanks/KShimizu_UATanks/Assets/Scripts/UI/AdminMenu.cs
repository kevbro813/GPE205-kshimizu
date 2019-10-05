using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Admin menu, currently used to reset scores and PlayerPrefs
public class AdminMenu : MonoBehaviour
{
    // Reset scores
    public void ResetScores()
    {
        GameManager.instance.highScores.Clear();
    }
    // Reset scores and player prefs
    public void OnReset()
    {
        ResetScores();
        ResetPlayerPrefs();
        GameManager.instance.scoreManager.InitializeValues();
    }
    // Reset PlayerPrefs
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    // Menu buttons
    public void Resume()
    {
        GameManager.instance.gameState = "resume";
    }
}
