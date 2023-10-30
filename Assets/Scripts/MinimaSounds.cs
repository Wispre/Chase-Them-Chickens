using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaSounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] Screams;
    public AudioClip[] Chicken;

    public void PlayMinimaScream()
    {
        var chosenScream = Screams[Random.Range(0, Screams.Length)];

        audioSource.clip = chosenScream;
        audioSource.Play();

    }
}
