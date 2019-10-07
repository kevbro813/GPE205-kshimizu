using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Manages player scoring using IComparable
[System.Serializable]
public class ScoreData : IComparable<ScoreData>
{
    [HideInInspector] public float savedScore;
    [HideInInspector] public string savedPlayerName;

    // Default ScoreManager
    public ScoreData()
    {
        savedScore = 0;
        savedPlayerName = "Player";
    }
    // Compare score to other scores
    public int CompareTo(ScoreData other)
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
    public void CopyFrom(ScoreData sd)
    {
        savedScore = sd.savedScore;
        savedPlayerName = sd.savedPlayerName;
    }
}
