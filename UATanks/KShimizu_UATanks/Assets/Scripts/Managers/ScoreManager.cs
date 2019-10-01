using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ScoreManager : IComparable<ScoreManager>
{
    public float savedScore;
    public string savedPlayerName;
    public ScoreManager()
    {
        savedScore = 0;
        savedPlayerName = "Player";
    }
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
    public void CopyFrom(ScoreManager sm)
    {
        savedScore = sm.savedScore;
        savedPlayerName = sm.savedPlayerName;
    }
}
