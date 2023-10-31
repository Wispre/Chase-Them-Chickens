using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour, IWorkInGameplay
{
    public GameManager manager;

    public float startTime;

    private float time;
    private TMP_Text timer;

    public void StartWorking()
    {
        time = startTime;
        timer.text = time.ToString("F0");
        StartCoroutine(StartTimer());
    }

    public void StopWorking()
    {
        time = startTime;
        StopAllCoroutines();
    }

    void Awake()
    {
        timer = GetComponent<TMP_Text>();
    }
    private void Start()
    {
        timer.text = time.ToString("F0");
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while(time > 0f)
        {
            time -= Time.deltaTime;
            timer.text = time.ToString("F0");
            yield return null;
        }
        manager.ShowGameOver();
    }
}
