using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action Reset;
    public Action OnGameOver;

    public GameStateChange[] Canvases;
    public GameObject[] ObjsWorkInGamePlay;

    public CinemachineVirtualCamera MenuCam;
    public CinemachineVirtualCamera GameplayCam;

    private List<IWorkInGameplay> workInGameplay = new List<IWorkInGameplay>();

    void Awake()
    {
        if(Instance == null) Instance = this;

        foreach (var work in ObjsWorkInGamePlay) 
        { 
            workInGameplay.Add(work.GetComponent<IWorkInGameplay>());
        }
    }

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        ChangeState(GameStates.MainMenu);
        MenuCam.Priority = 20;
        GameplayCam.Priority = 10;
        WorkInGameplay(false);
    }

    public void ShowTutorial()
    {
        ChangeState(GameStates.Tutorial);
        MenuCam.Priority = 20;
        GameplayCam.Priority = 10;
        PlayerPrefs.SetInt(GlobalConsts.COMPLETED_TUTORIAL, 1);
        WorkInGameplay(false);
    }

    public void ShowGamePlay()
    {
        if (PlayerPrefs.GetInt(GlobalConsts.COMPLETED_TUTORIAL, 0) == 0)
        {
            ShowTutorial();
            return;
        }
        ChangeState(GameStates.GamePlay);
        MenuCam.Priority = 10;
        GameplayCam.Priority = 20;
        WorkInGameplay(true);
        Instance.Reset?.Invoke();
    }

    public void ShowGameOver()
    {
        Instance.OnGameOver?.Invoke();
        ChangeState(GameStates.GameOver);
        MenuCam.Priority = 20;
        GameplayCam.Priority = 10;
        WorkInGameplay(false);
    }

    private void ChangeState(GameStates changeTo)
    {
        foreach (GameStateChange state in Canvases)
        {
            state.EnableIfChosen(changeTo);
        }
    }

    private void WorkInGameplay(bool inGameplay)
    {
        if (inGameplay)
        {
            foreach (IWorkInGameplay obj in workInGameplay)
            {
                obj.StartWorking();
            }
        }
        else
        {
            foreach (IWorkInGameplay obj in workInGameplay)
            {
                obj.StopWorking();
            }
        }
    }
}
