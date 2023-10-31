using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour, IWorkInGameplay
{
    public PlayerInput input;

    public void StartWorking()
    {
        input.enabled = true;
    }

    public void StopWorking()
    {
        input.enabled = false;
    }
}
