using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public ScoreManager scoreManager;
    public TMP_Text scoreText;


    void OnEnable()
    {
        scoreText.text = scoreManager.score.ToString();
    }
}
