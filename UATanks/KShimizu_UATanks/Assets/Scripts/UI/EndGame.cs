using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public Text playerOneName;
    public Text playerTwoName;
    public Text playerOneScore;
    public Text playerTwoScore;

    public Text highScoreOneName;
    public Text highScoreTwoName;
    public Text highScoreThreeName;
    public Text highScoreFourName;
    public Text highScoreFiveName;
    public Text highScoreSixName;
    public Text highScoreSevenName;
    public Text highScoreEightName;
    public Text highScoreNineName;
    public Text highScoreTenName;
    public Text highScoreOneScore;
    public Text highScoreTwoScore;
    public Text highScoreThreeScore;
    public Text highScoreFourScore;
    public Text highScoreFiveScore;
    public Text highScoreSixScore;
    public Text highScoreSevenScore;
    public Text highScoreEightScore;
    public Text highScoreNineScore;
    public Text highScoreTenScore;

    public Text Winner;

    private void Update()
    {
        if (GameManager.instance.isMultiplayer == true)
        {
            playerOneName.text = GameManager.instance.playerData[0].playerName;
            playerOneScore.text = GameManager.instance.playerData[0].score.ToString();
            playerTwoName.text = GameManager.instance.playerData[1].playerName;
            playerTwoScore.text = GameManager.instance.playerData[1].score.ToString();

            if (GameManager.instance.activeEnemiesList.Count == 0)
            {
                if (GameManager.instance.playerData[0].score > GameManager.instance.playerData[1].score)
                {
                    Winner.text = GameManager.instance.playerData[0].playerName + " is the Winner!";
                }
                else if (GameManager.instance.playerData[0].score < GameManager.instance.playerData[1].score)
                {
                    Winner.text = GameManager.instance.playerData[1].playerName + " is the Winner!";
                }
                else if (GameManager.instance.playerData[0].score == GameManager.instance.playerData[1].score)
                {
                    Winner.text = "We have a tie!";
                }
            }
            else
            {
                Winner.text = "Both Players Lose...";
            }
        }
        else
        {
            playerOneName.text = GameManager.instance.playerData[0].playerName;
            playerOneScore.text = GameManager.instance.playerData[0].score.ToString();
            playerTwoName.text = "";
            playerTwoScore.text = "";

            if (GameManager.instance.activeEnemiesList.Count == 0)
            {
                Winner.text = GameManager.instance.playerData[0].playerName + " Wins!";
            }
            else
            {
                Winner.text = GameManager.instance.playerData[0].playerName + " Loses!";
            }
        }

        highScoreOneName.text = GameManager.instance.highScores[0].savedPlayerName;
        highScoreOneScore.text = GameManager.instance.highScores[0].savedScore.ToString();
        highScoreTwoName.text = GameManager.instance.highScores[1].savedPlayerName;
        highScoreTwoScore.text = GameManager.instance.highScores[1].savedScore.ToString();
        highScoreThreeName.text = GameManager.instance.highScores[2].savedPlayerName;
        highScoreThreeScore.text = GameManager.instance.highScores[2].savedScore.ToString();
        highScoreFourName.text = GameManager.instance.highScores[3].savedPlayerName;
        highScoreFourScore.text = GameManager.instance.highScores[3].savedScore.ToString();
        highScoreFiveName.text = GameManager.instance.highScores[4].savedPlayerName;
        highScoreFiveScore.text = GameManager.instance.highScores[4].savedScore.ToString();
        highScoreSixName.text = GameManager.instance.highScores[5].savedPlayerName;
        highScoreSixScore.text = GameManager.instance.highScores[5].savedScore.ToString();
        highScoreSevenName.text = GameManager.instance.highScores[6].savedPlayerName;
        highScoreSevenScore.text = GameManager.instance.highScores[6].savedScore.ToString();
        highScoreEightName.text = GameManager.instance.highScores[7].savedPlayerName;
        highScoreEightScore.text = GameManager.instance.highScores[7].savedScore.ToString();
        highScoreNineName.text = GameManager.instance.highScores[8].savedPlayerName;
        highScoreNineScore.text = GameManager.instance.highScores[8].savedScore.ToString();
        highScoreTenName.text = GameManager.instance.highScores[9].savedPlayerName;
        highScoreTenScore.text = GameManager.instance.highScores[9].savedScore.ToString();
    }
    public void Menu()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        GameManager.instance.gameState = "menu";
    }
    public void Quit()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        GameManager.instance.gameState = "quit";
    }
}
