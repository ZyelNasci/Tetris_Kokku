using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private int currentScore;
    private int highScore;

    public int RowValue;

    public int fallValue;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = "HIGH SCORE: \n" + highScore.ToString("00000000");
        UIController.Instance.StopEvent += StopGameEvent;
        UIController.Instance.StartEvent += StartGameEvent;
    }

    public void StartGameEvent()
    {
        currentScore = 0;
        scoreText.text = "SCORE: \n" + currentScore.ToString("00000000");
    }

    public void StopGameEvent()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            highScoreText.text = "HIGH SCORE: \n" + highScore.ToString("00000000");
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    public void AddPoints(int rowCount)
    {
        currentScore += RowValue * rowCount;
        scoreText.text = "SCORE: " + currentScore.ToString("00000000");
    }
}
