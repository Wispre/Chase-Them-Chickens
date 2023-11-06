using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IWorkInGameplay
{
    public int score { get; private set; }

    public void IncreaseScore()
    {
        score++;
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt(GlobalConsts.CURRENT_SCORE, score);

        if (score > PlayerPrefs.GetInt(GlobalConsts.HIGH_SCORE, 0))
        {
            PlayerPrefs.SetInt(GlobalConsts.HIGH_SCORE, score);
        }

        print($"score saved {PlayerPrefs.GetInt(GlobalConsts.CURRENT_SCORE)}            {PlayerPrefs.GetInt(GlobalConsts.HIGH_SCORE)}");
    }

    void Awake()
    {
        GameManager.Instance.OnGameOver += SaveScore;
    }

    void OnEnable()
    {
        PlayerPrefs.SetFloat(GlobalConsts.CURRENT_SCORE, 0);
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(GlobalConsts.CURRENT_SCORE, score);
    }

    public void StartWorking()
    {
        score = 0;
    }

    public void StopWorking()
    {
    }
}
