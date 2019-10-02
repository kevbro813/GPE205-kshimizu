using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public ScoreData scoreData; // Score Manager component
    // Initialize ScoreManager
    public void InitializeValues()
    {
        scoreData = new ScoreData();
    }
    // Fill any empty scores (prevents "index out of range" error)
    public void FillEmptyScores()
    {
        // Loops through until highScores.Count = maxScores
        for (int i = GameManager.instance.highScores.Count; i < GameManager.instance.maxScores; i++)
        {
            // Adds a placeholder score
            ScoreData sm = new ScoreData();
            GameManager.instance.highScores.Add(sm);
        }
    }
    // Run functions to save scores
    public void OnSave()
    {
        SetScores();
        CheckScores();
        SavePlayerScores();
    }
    // Sort high scores, reverse order and limit quantity of scores to maxScores
    public void CheckScores()
    {
        GameManager.instance.highScores.Sort();
        GameManager.instance.highScores.Reverse();

        int numScores = Mathf.Min(GameManager.instance.maxScores, GameManager.instance.highScores.Count);
        GameManager.instance.highScores = GameManager.instance.highScores.GetRange(0, numScores);
    }
    // Adds new scores to highScores
    public void SetScores()
    {
        // Loop through active playerData and add playerName and scores to currentScores (needed to use scoreManager)
        for (int i = 0; i < GameManager.instance.playerData.Count; i++)
        {
            ScoreData sd = new ScoreData();
            GameManager.instance.currentScores.Add(sd);
            GameManager.instance.currentScores[i].savedScore = GameManager.instance.playerData[i].score;
            GameManager.instance.currentScores[i].savedPlayerName = GameManager.instance.playerData[i].playerName;
        }
        // Loop through currentScores and add them to the list of high scores
        for (int j = 0; j < GameManager.instance.currentScores.Count; j++)
        {
            ScoreData sd = new ScoreData();
            GameManager.instance.highScores.Add(sd);
            GameManager.instance.highScores[GameManager.instance.highScores.Count - 1].savedScore = GameManager.instance.currentScores[j].savedScore;
            GameManager.instance.highScores[GameManager.instance.highScores.Count - 1].savedPlayerName = GameManager.instance.currentScores[j].savedPlayerName;
        }
    }
    // Saves player scores to PlayerPrefs
    public void SavePlayerScores()
    {
        for (int i = 0; i < GameManager.instance.highScores.Count; i++)
        {
            ScoreData sd = GameManager.instance.highScores[i];
            PlayerPrefs.SetFloat("Score" + i.ToString(), sd.savedScore);
            PlayerPrefs.SetString("PlayerName" + i.ToString(), sd.savedPlayerName);
        }
        PlayerPrefs.Save();
    }
    // Load score data from PlayerPrefs
    public void LoadPlayerScores()
    {
        string key;
        for (int i = 0; i < GameManager.instance.maxScores; i++)
        {
            Debug.Log("Loading Player Scores");
            ScoreData sd = new ScoreData();
            key = "Score" + i.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                sd.savedScore = PlayerPrefs.GetFloat(key);
            }
            else
            {
                break;
            }
            key = "PlayerName" + i.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                sd.savedPlayerName = PlayerPrefs.GetString(key);
            }
            else
            {
                break;
            }
            GameManager.instance.highScores.Add(sd);
        }
    }
}
