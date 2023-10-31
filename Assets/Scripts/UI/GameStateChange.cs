using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateChange : MonoBehaviour
{
    public GameStates State;
    [Space(5)]
    public GameObject[] objects;


    public void EnableIfChosen(GameStates state)
    {
        if(state == this.State)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    private void Enable()
    {
        foreach(GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }

    private void Disable()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
}
