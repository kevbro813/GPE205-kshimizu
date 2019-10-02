using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Manages player scoring using IComparable
[System.Serializable]
public class ScoreManager : IComparable<ScoreManager>
{
    public float savedScore;
    public string savedPlayerName;

    // Default ScoreManager
    public ScoreManager()
    {
        savedScore = 0;
        savedPlayerName = "Player";
    }
    // Compare score to other scores
    public int CompareTo(ScoreManager other)
    {
        if (other == null)
        {
            return 1;
        }
        if (this.savedScore > other.savedScore)
        {
            return 1;
        }
        if (this.savedScore < other.savedScore)
        {
            return -1;
        }
        return 0;
    }
    // Copy score to component
    public void CopyFrom(ScoreManager sm)
    {
        savedScore = sm.savedScore;
        savedPlayerName = sm.savedPlayerName;
    }
}
