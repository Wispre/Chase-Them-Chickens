using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score { get; private set; }

    public void IncreaseScore()
    {
        score++;
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt(GlobalConsts.CURRENT_SCORE, score);
    }

    void OnEnable()
    {
        PlayerPrefs.SetFloat(GlobalConsts.CURRENT_SCORE, 0);
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(GlobalConsts.CURRENT_SCORE, score);
    }
}
