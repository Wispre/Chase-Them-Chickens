using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
}
