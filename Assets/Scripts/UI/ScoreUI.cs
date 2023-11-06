using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public ScoreManager scoreManager;
    public TMP_Text scoreText;

    public bool isHighScore;


    void OnEnable()
    {
        if (isHighScore)
        {
            scoreText.text = PlayerPrefs.GetInt(GlobalConsts.HIGH_SCORE, 0).ToString();
        }
        else
        {
            scoreText.text = scoreManager.score.ToString();
        }
        
    }
}
